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
            uncatchable = value;
            View.RPC("UpdateUncatchable", RPCTargets.Others, value);
            MakeBallUncatchable(value);
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
    private void UpdateUncatchable(bool uncatchable)
    {
        this.uncatchable = uncatchable;
        MakeBallUncatchable(uncatchable);
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
    public void Detach()
    {
        if (NO_PLAYER_ID != IdPlayerOwningBall)
        {
            ConnectionId previousId = IdPlayerOwningBall;
            IdPlayerOwningBall = NO_PLAYER_ID;
            MyComponents.Players.ChangePlayerOwningBall(previousId, NO_PLAYER_ID, true);
        }
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
            Assert.IsTrue(GetAttachedPlayer() != null);
            Transform hand = GetAttachedPlayer().PlayerMesh.hand;
            UnCatchable = false;
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
