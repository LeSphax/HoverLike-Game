using Byn.Net;
using PlayerManagement;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class BallPhysicsModel : PhysicsModel
{
    CustomRigidbody myRigidbody;

    private ConnectionId playerOwningBall = Players.INVALID_PLAYER_ID;
    public ConnectionId PlayerOwningBall
    {
        get
        {
            return playerOwningBall;
        }
        set
        {
            playerOwningBall = value;
        }
    }

    public bool IsAttached
    {
        get
        {
            return playerOwningBall != Players.INVALID_PLAYER_ID;
        }
    }

    public float MAX_SPEED = 200;

    public override void Simulate(short frameNumber, float dt, bool isRealSimulation)
    {
        myRigidbody.Simulate(dt);
    }

    protected  void Awake()
    {
        myRigidbody = gameObject.GetComponent<CustomRigidbody>();
    }

    public override byte[] Serialize()
    {
        byte[] data = BitConverter.GetBytes(IsAttached);
        if (IsAttached)
        {
            data = ArrayExtensions.Concatenate(data, BitConverter.GetBytes(PlayerOwningBall.id));
            return data;
        }
        else
        {
            data = ArrayExtensions.Concatenate(data, BitConverterExtensions.GetBytes(myRigidbody.velocity));
            data = ArrayExtensions.Concatenate(data, BitConverterExtensions.GetBytes(gameObject.transform.position));
            return data;
        }
    }

    public override int DeserializeAndRewind(byte[] data, int currentIndex)
    {
        bool isAttached = BitConverter.ToBoolean(data, currentIndex);
        currentIndex++;
        if (!isAttached)
        {
            if (playerOwningBall != Players.INVALID_PLAYER_ID)
                UpdatePlayerOwningBall(Players.INVALID_PLAYER_ID);
            myRigidbody.velocity = BitConverterExtensions.ToVector3(data, currentIndex);
            currentIndex += 12;
            gameObject.transform.position = BitConverterExtensions.ToVector3(data, currentIndex);
            return 25;
        }
        else
        {
            UpdatePlayerOwningBall(new ConnectionId(BitConverter.ToInt16(data, currentIndex)));
            return 3;
        }
    }

    private void UpdatePlayerOwningBall(ConnectionId newPlayerId)
    {
        ConnectionId oldPlayerOwningBall = playerOwningBall;
        playerOwningBall = newPlayerId;
        MyComponents.Players.PlayerOwningBallChanged(oldPlayerOwningBall, playerOwningBall);
    }

    public void Throw(Vector3 target, float power, float latencyinSeconds)
    {
        myRigidbody.Reset();
        Vector3 velocity = new Vector3(target.x - transform.position.x, 0, target.z - transform.position.z);
        velocity.Normalize();
        myRigidbody.velocity = velocity * MAX_SPEED * Mathf.Max(power, 0.3f);
        //ballModel.transform.position = transform.position + myRigidbody.velocity * latencyinSeconds;
    }

    public override void CheckForPostSimulationActions()
    {

    }

    public override void CheckForPreSimulationActions()
    {
    }

    public override byte[] SerializeInputs(short frame)
    {
        return new byte[0];
    }

    internal override void RemoveAcknowledgedInputs(short lastAckFrame, short ackFrame)
    {
    }
}
