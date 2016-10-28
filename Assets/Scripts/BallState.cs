using System;
using UnityEngine;

public class BallState : SlideBall.MonoBehaviour
{
    private const int NO_PLAYER_ID = -1;

    //This is set to false when we want the ball's simulation to be handled by the client
    public bool ListenToServer = true;
    public GameObject Ball;
    public static Vector3 ballHoldingPosition = new Vector3(0f, 0f, 4f);


    void Awake()
    {
        Ball = gameObject;
        MyGameObjects.GameInitialization.AddGameStartedListener(StartGame);
    }

    void Start()
    {
        MyGameObjects.Properties.AddListener(PropertiesKeys.NamePlayerHoldingBall, (previousValue, value) => ListenToServer = true);
    }

    public void StartGame()
    {
        if (MyGameObjects.NetworkManagement.isServer)
        {
            MyGameObjects.Properties.SetProperty(PropertiesKeys.NamePlayerHoldingBall, NO_PLAYER_ID);
        }

        AttachBall(GetIdOfPlayerOwningBall());

    }

    public void SetAttached(int playerID)
    {
        MyGameObjects.Properties.SetProperty(PropertiesKeys.NamePlayerHoldingBall, playerID);

    }
    public void Detach()
    {
        MyGameObjects.Properties.SetProperty(PropertiesKeys.NamePlayerHoldingBall, NO_PLAYER_ID);
    }

    public bool IsAttached()
    {
        return GetIdOfPlayerOwningBall() != NO_PLAYER_ID;
    }

    public int GetIdOfPlayerOwningBall()
    {
        object attachedPlayerID;
        MyGameObjects.Properties.TryGetProperty(PropertiesKeys.NamePlayerHoldingBall, out attachedPlayerID);
        if (attachedPlayerID == null)
        {
            //Debug.LogError("The attachedPlayer wasn't set, this should not happen");
            return NO_PLAYER_ID;
        }
        else
            return (int)attachedPlayerID;
    }

    public GameObject GetAttachedPlayer()
    {
        foreach (GameObject player in Tags.FindPlayers())
        {
            if (player.GetNetworkView().ViewId == GetIdOfPlayerOwningBall())
                return player;
        }
        return null;
    }


    public void AttachBall(int viewId)
    {
        bool attach = viewId != NO_PLAYER_ID;
        Debug.Log("Call To Attach Ball " + viewId + "   " + attach);
        GameObject player = GetAttachedPlayer();
        if (attach)
        {
            Debug.Log("Attach Ball" + Ball + "    " + player);
            Ball.transform.SetParent(player.transform);
            Ball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            Ball.transform.localPosition = ballHoldingPosition;
        }
        else
        {
            Debug.Log("Detach Ball");
            //Physics.IgnoreCollision(ball.GetComponent<Collider>(), player.GetComponent<Collider>(),);
            Ball.transform.SetParent(null);
            Ball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }

    [MyRPC]
    internal void PutAtStartPosition()
    {
        if (MyGameObjects.NetworkManagement.isServer)
        {
            SetAttached(NO_PLAYER_ID);
            View.RPC("PutAtStartPosition", RPCTargets.Others);
        }
        Ball.transform.position = MyGameObjects.Spawns.BallSpawn;
        Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
