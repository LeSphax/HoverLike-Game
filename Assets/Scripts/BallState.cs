using Byn.Net;
using PlayerManagement;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public delegate void UncatchableChangeHandler(bool uncatchable);

public class BallState : SlideBall.MonoBehaviour
{
    public event UncatchableChangeHandler UncatchableChanged;

    public static ConnectionId NO_PLAYER_ID
    {
        get
        {
            return Players.INVALID_PLAYER_ID;
        }
    }

    public ConnectionId IdPlayerOwningBall = NO_PLAYER_ID;

    public ConnectionId PassTarget = NO_PLAYER_ID;

    private bool uncatchable;
    public bool UnCatchable
    {
        get
        {
            return uncatchable;
        }
        set
        {
            View.RPC("UpdateUncatchable", RPCTargets.All, value);
        }
    }

    private void MakeBallUncatchable(bool uncatchable)
    {
        if (uncatchable)
        {
            protectionSphere.SetActive(true);
        }
        else
        {
            protectionSphere.SetActive(false);
        }
        TrySetKinematic();

    }

    private void TrySetKinematic()
    {
        if (MyComponents.NetworkManagement.isServer)
        {
            if (uncatchable || IsAttached())
                GetComponent<Rigidbody>().isKinematic = true;
            else
                GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    [MyRPC]
    private void UpdateUncatchable(bool newValue)
    {
        this.uncatchable = newValue;
        MakeBallUncatchable(newValue);
        if (UncatchableChanged != null)
            UncatchableChanged.Invoke(newValue);
    }

    [SerializeField]
    private GameObject protectionSphere;

    public static Vector3 ballHoldingPosition = new Vector3(0f, 0f, 0f);

    void Awake()
    {
        MyComponents.GameInitialization.AddGameStartedListener(StartGame);
    }

    void Start()
    {
        UnCatchable = false;
    }

    public void StartGame()
    {
        if (MyComponents.NetworkManagement.isServer)
        {
            IdPlayerOwningBall = NO_PLAYER_ID;
        }
        AttachBall(GetIdOfPlayerOwningBall());
    }

    public void SetAttached(ConnectionId playerId, bool sendUpdate = true)
    {
        if (playerId != IdPlayerOwningBall)
        {
            ConnectionId previousId = IdPlayerOwningBall;
            IdPlayerOwningBall = playerId;
            MyComponents.Players.ChangePlayerOwningBall(previousId, playerId, sendUpdate);
        }
    }
    public void DetachBall()
    {
        SetAttached(NO_PLAYER_ID);
    }

    public bool IsAttached()
    {
        return GetIdOfPlayerOwningBall() != NO_PLAYER_ID;
    }

    public ConnectionId GetIdOfPlayerOwningBall()
    {
        return IdPlayerOwningBall;
    }

    public PlayerController GetAttachedPlayer()
    {
        foreach (Player player in Players.players.Values)
        {
            if (player.id == GetIdOfPlayerOwningBall())
                return player.controller;
        }
        return null;
    }


    public void AttachBall(ConnectionId playerId)
    {
        bool attach = playerId != NO_PLAYER_ID;
        if (attach)
        {
            if (MyComponents.NetworkManagement.isServer)
                UnCatchable = false;
            Assert.IsTrue(GetAttachedPlayer() != null);
            Transform hand = GetAttachedPlayer().PlayerMesh.hand;
            gameObject.transform.SetParent(hand);
            TrySetKinematic();
            gameObject.transform.localPosition = ballHoldingPosition;
        }
        else
        {
            gameObject.transform.SetParent(null);
            TrySetKinematic();
        }
    }

    [MyRPC]
    internal void PutAtStartPosition()
    {
        if (MyComponents.NetworkManagement.isServer)
        {
            DetachBall();
            View.RPC("PutAtStartPosition", RPCTargets.Others);
        }
        PutBallAtPosition(MyComponents.Spawns.BallSpawn);
    }

    public void PutBallAtPosition(Vector3 position)
    {
        gameObject.transform.position = position;
        if (MyComponents.NetworkManagement.isServer)
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.GetComponentInChildren<AttractionBall>().Reset();
        }
    }
}
