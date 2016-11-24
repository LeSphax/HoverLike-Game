using Byn.Net;
using PlayerManagement;
using System;
using UnityEngine;

public class BallState : SlideBall.MonoBehaviour
{
    public static ConnectionId NO_PLAYER_ID
    {
        get
        {
            return Players.INVALID_PLAYER_ID;
        }
    }

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
            if (MyComponents.NetworkManagement.isServer)
            {
                GetComponent<Rigidbody>().detectCollisions = false;
                GetComponent<Rigidbody>().isKinematic = true;
            }
            protectionSphere.SetActive(true);
        }
        else
        {
            if (MyComponents.NetworkManagement.isServer)
            {
                GetComponent<Rigidbody>().detectCollisions = true;
                GetComponent<Rigidbody>().isKinematic = false;
            }
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
        GameObject player = GetAttachedPlayer();
        if (attach)
        {
            gameObject.transform.SetParent(player.transform);
            if (MyComponents.NetworkManagement.isServer)
                gameObject.GetComponent<Rigidbody>().isKinematic = true;
            gameObject.transform.localPosition = ballHoldingPosition;
        }
        else
        {
            gameObject.transform.SetParent(null);
            if (MyComponents.NetworkManagement.isServer)
                gameObject.GetComponent<Rigidbody>().isKinematic = false;
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
        if (MyComponents.NetworkManagement.isServer)
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.GetComponentInChildren<AttractionBall>().Reset();
        }
    }
}
