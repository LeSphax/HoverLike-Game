using Byn.Net;
using PlayerManagement;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class BallState : SlideBall.MonoBehaviour
{
    public static ConnectionId NO_PLAYER_ID
    {
        get
        {
            return Players.INVALID_PLAYER_ID;
        }
    }

    private CustomRigidbody myRigidbody;

    public GameObject ballModel;
    public GameObject ballView;
    [HideInInspector]
    public BallPhysicsModel ballPhysics;

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
            myRigidbody.activated = false;
            protectionSphere.SetActive(true);
        }
        else
        {
            myRigidbody.activated = true;
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
        myRigidbody = ballModel.GetComponent<CustomRigidbody>();
        ballPhysics = ballModel.GetComponent<BallPhysicsModel>();
    }

    void Start()
    {
        Uncatchable = false;
    }

    public void SetAttached(ConnectionId playerId)
    {
        ConnectionId oldPlayerId = ballPhysics.PlayerOwningBall;
        ballPhysics.PlayerOwningBall = playerId;
        MyComponents.Players.PlayerOwningBallChanged(oldPlayerId, playerId);
    }

    public bool IsAttached()
    {
        return ballPhysics.IsAttached;
    }

    public ConnectionId GetIdOfPlayerOwningBall()
    {
        return ballPhysics.PlayerOwningBall;
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
            ballModel.transform.SetParent(player.GetComponent<PlayerController>().physicsModel.transform);
            ballView.transform.SetParent(player.GetComponent<PlayerController>().physicsView.transform);
            myRigidbody.activated = false;
            ballModel.transform.localPosition = ballHoldingPosition;
            ballView.transform.localPosition = ballHoldingPosition;
        }
        else
        {
            ballModel.transform.SetParent(null);
            ballView.transform.SetParent(null);
            myRigidbody.activated = true;
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
        ballModel.transform.position = MyComponents.Spawns.BallSpawn;
        myRigidbody.velocity = Vector3.zero;
        ballModel.GetComponentInChildren<AttractionBall>().Reset();
    }
}
