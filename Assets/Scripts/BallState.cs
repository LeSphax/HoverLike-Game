using Byn.Net;
using PlayerManagement;
using System;
using UnityEngine;

public class BallState : SlideBall.MonoBehaviour
{
    public static ConnectionId NO_PLAYER_ID = new ConnectionId(-100);

    //This is set to false when we want the ball's simulation to be handled by the client
    [HideInInspector]
    public bool ListenToServer = true;

    private bool uncatchable;
    public bool Uncatchable
    {
        get
        {
            return uncatchable;
        }
        set
        {
            uncatchable = value;
            View.RPC("UpdateUncatchable", RPCTargets.Others, value);
            MakeBallUncatchable(value);
        }
    }

    private void MakeBallUncatchable(bool uncatchable)
    {
        if (uncatchable)
        {
            GetComponent<Rigidbody>().Sleep();
            protectionSphere.SetActive(true);
        }
        else
        {
            GetComponent<Rigidbody>().WakeUp();
            protectionSphere.SetActive(false);
        }
    }

    [MyRPC]
    private void UpdateUncatchable(bool uncatchable)
    {
        this.uncatchable = uncatchable;
        MakeBallUncatchable(uncatchable);
    }

    [SerializeField]
    private GameObject protectionSphere;

    public static Vector3 ballHoldingPosition = new Vector3(0f, 0f, 4f);

    void Awake()
    {
        MyComponents.GameInitialization.AddGameStartedListener(StartGame);
    }

    void Start()
    {
        MyComponents.Properties.AddListener(PropertiesKeys.IdPlayerOwningBall, (previousValue, value) => ListenToServer = true);
        Uncatchable = false;
    }

    public void StartGame()
    {
        if (MyComponents.NetworkManagement.isServer)
        {
            MyComponents.Properties.SetProperty(PropertiesKeys.IdPlayerOwningBall, NO_PLAYER_ID);
        }

        AttachBall(GetIdOfPlayerOwningBall());

    }

    public void SetAttached(ConnectionId playerID)
    {
        MyComponents.Properties.SetProperty(PropertiesKeys.IdPlayerOwningBall, playerID);

    }
    public void Detach()
    {
        MyComponents.Properties.SetProperty(PropertiesKeys.IdPlayerOwningBall, NO_PLAYER_ID);
    }

    public bool IsAttached()
    {
        return GetIdOfPlayerOwningBall() != NO_PLAYER_ID;
    }

    public ConnectionId GetIdOfPlayerOwningBall()
    {
        object attachedPlayerID;
        MyComponents.Properties.TryGetProperty(PropertiesKeys.IdPlayerOwningBall, out attachedPlayerID);
        if (attachedPlayerID == null)
        {
            //Debug.LogError("The attachedPlayer wasn't set, this should not happen");
            return NO_PLAYER_ID;
        }
        else
            return (ConnectionId)attachedPlayerID;
    }

    public GameObject GetAttachedPlayer()
    {
        foreach (Player player in Players.players.Values)
        {
            if (player.id == GetIdOfPlayerOwningBall())
                return player.controller.gameObject;
        }
        return null;
    }


    public void AttachBall(ConnectionId playerId)
    {
        bool attach = playerId != NO_PLAYER_ID;
        Debug.Log("Call To Attach Ball " + playerId + "   " + attach);
        GameObject player = GetAttachedPlayer();
        if (attach)
        {
            gameObject.transform.SetParent(player.transform);
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            gameObject.transform.localPosition = ballHoldingPosition;
        }
        else
        {
            gameObject.transform.SetParent(null);
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }

    [MyRPC]
    internal void PutAtStartPosition()
    {
        if (MyComponents.NetworkManagement.isServer)
        {
            SetAttached(NO_PLAYER_ID);
            View.RPC("PutAtStartPosition", RPCTargets.Others);
        }
        gameObject.transform.position = MyComponents.Spawns.BallSpawn;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.GetComponentInChildren<AttractionBall>().Reset();
    }
}
