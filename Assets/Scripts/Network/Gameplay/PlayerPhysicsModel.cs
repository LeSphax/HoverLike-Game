using Byn.Net;
using PlayerBallControl;
using PlayerManagement;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public delegate void PacketHandler(byte[] data);

[RequireComponent(typeof(CustomRigidbody))]
public class PlayerPhysicsModel : PhysicsModel
{
    private ConnectionId? playerConnectionId = null;
    private Player Player
    {
        get
        {
            Assert.IsTrue(playerConnectionId != null);
            return PlayerView.GetMyPlayer(playerConnectionId.Value);
        }
    }
    [SerializeField]
    private PlayerController controller;
    private PlayerBallController ballController;
    private PlayerMovementStrategy strategy;

    private CustomRigidbody myRigidbody;

    internal Vector3? targetPosition
    {
        set
        {
            strategy.targetPosition = value;
        }
    }

    private float SimulationTime
    {
        get
        {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
            return MyComponents.TimeManagement.NetworkTimeInSeconds - ClientDelay.Delay;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        myRigidbody = GetComponent<CustomRigidbody>();
        ballController = controller.GetComponent<PlayerBallController>();
    }


    public void Reset(ConnectionId connectionId)
    {
        playerConnectionId = connectionId;
        if (strategy != null)
        {
            Destroy(strategy);
        }
        switch (Player.AvatarSettingsType)
        {
            case AvatarSettings.AvatarSettingsTypes.GOALIE:
                strategy = gameObject.AddComponent<GoalieMovementStrategy>();
                break;
            case AvatarSettings.AvatarSettingsTypes.ATTACKER:
                strategy = gameObject.AddComponent<AttackerMovementStrategy>();
                break;
            default:
                throw new UnhandledSwitchCaseException(Player.AvatarSettingsType);
        }
    }

    public override void Simulate(float dt)
    {
        strategy.UpdateMovement();
        myRigidbody.Simulate(Time.fixedDeltaTime);
    }

    public override byte[] Serialize()
    {
        byte[] data;
        if (strategy.targetPosition != null)
        {
            data = BitConverter.GetBytes(true);
            data = ArrayExtensions.Concatenate(data, BitConverterExtensions.GetBytes(strategy.targetPosition.Value));
        }
        else
        {
            data = BitConverter.GetBytes(false);
        }
        data = ArrayExtensions.Concatenate(data, BitConverterExtensions.GetBytes(myRigidbody.velocity));
        data = ArrayExtensions.Concatenate(data, BitConverterExtensions.GetBytes(transform.position));
        data =  ArrayExtensions.Concatenate(data, BitConverter.GetBytes(transform.rotation.y));
        return data;
    }

    public override int DeserializeAndRewind(byte[] data, int currentIndex)
    {
        Debug.LogWarning(data.Length + "   " + currentIndex);
        int offset = 0;
        bool hasTarget = BitConverter.ToBoolean(data, currentIndex + offset);
        offset += 1;
        if (hasTarget)
        {
            targetPosition = BitConverterExtensions.ToVector3(data, currentIndex + offset);
            offset += 12;
        }
        else
        {
            targetPosition = null;
        }
        myRigidbody.velocity = BitConverterExtensions.ToVector3(data, currentIndex + offset);
        offset += 12;
        Debug.LogWarning(currentIndex + offset);
        gameObject.transform.position = BitConverterExtensions.ToVector3(data, currentIndex + offset);
        offset += 12;
        gameObject.transform.rotation = Quaternion.Euler(Vector3.up * BitConverter.ToSingle(data, currentIndex + offset));
        offset += 4;
        return offset;
    }

    public override void CheckForPostSimulationActions()
    {
        ballController.TryCatchBall(transform.position);
    }

    public override void CheckForPreSimulationActions()
    {
        ballController.TryAttractBall(transform.position);
    }

}

