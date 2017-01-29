using Byn.Net;
using PlayerManagement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public delegate void PacketHandler(byte[] data);

[RequireComponent(typeof(PlayerController))]
public class PlayerMovementManager : ObservedComponent
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

    private PlayerMovementStrategy strategy;
    internal PlayerController controller;

    private Queue<PlayerPacket> StateBuffer = new Queue<PlayerPacket>();
    private PlayerPacket? currentPacket = null;


    private PlayerPacket? nextPacket
    {
        get
        {
            if (StateBuffer.Count > 0)
            {
                return StateBuffer.Peek();
            }
            else
            {
                return null;
            }
        }
    }

    private float SimulationTime
    {
        get
        {
            return TimeManagement.NetworkTimeInSeconds - ClientDelay.Delay;
        }
    }

    internal Vector3? targetPosition
    {
        get
        {
            return strategy.TargetPosition;
        }
        set
        {
            strategy.TargetPosition = value;
        }
    }



    protected virtual void Awake()
    {
        controller = GetComponent<PlayerController>();
        strategy = gameObject.AddComponent<NotInitializedStrategy>();
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


    public override void OwnerUpdate()
    {
        controller.abilitiesManager.ApplyAbilityEffects(Time.fixedDeltaTime);
        strategy.UpdateMovement();
    }

    protected override byte[] CreatePacket(long sendId)
    {
        return new PlayerPacket(transform.position, transform.rotation,TimeManagement.NetworkTimeInSeconds).Serialize();
    }

    public override void SimulationUpdate()
    {
        while (StateBuffer.Count > 0 && SimulationTime >= StateBuffer.Peek().timeSent)
        {
            currentPacket = StateBuffer.Dequeue();
        }

        if (currentPacket != null)
        {
            InterpolateMovement();
        }
    }

    private void InterpolateMovement()
    {
        if (nextPacket != null)
        {
            float completion = (SimulationTime - currentPacket.Value.timeSent) / (nextPacket.Value.timeSent - currentPacket.Value.timeSent);
            transform.position = Vector3.Lerp(currentPacket.Value.position, nextPacket.Value.position, completion);
            transform.rotation = Quaternion.Lerp(currentPacket.Value.rotation, nextPacket.Value.rotation, completion);
        }
        else
        {
            transform.position = currentPacket.Value.position;
            transform.rotation = currentPacket.Value.rotation;
        }
    }

    public override void PacketReceived(ConnectionId id, byte[] data)
    {
        PlayerPacket newPacket = PlayerPacket.Deserialize(data);
        StateBuffer.Enqueue(newPacket);
    }

    protected override bool IsSendingPackets()
    {
        return MyComponents.NetworkManagement.isServer;
    }

    public void Brake()
    {
        Assert.IsTrue(strategy.GetType() == typeof(AttackerMovementStrategy));
        ((AttackerMovementStrategy)strategy).Brake();
    }

    public void Jump()
    {
        Assert.IsTrue(strategy.GetType() == typeof(AttackerMovementStrategy));
        ((AttackerMovementStrategy)strategy).Jump();
    }
}

public struct PlayerPacket
{
    public Vector3 position;
    public Quaternion rotation;
    public float timeSent;

    public PlayerPacket(Vector3 position, Quaternion rotation, float timeSent)
    {
        this.position = position;
        this.rotation = rotation;
        this.timeSent = timeSent;
    }

    public byte[] Serialize()
    {
        byte[] data = NetworkExtensions.SerializeVector3(position);
        data = ArrayExtensions.Concatenate(data,NetworkExtensions.SerializeQuaternion(rotation));
        return data.Concatenate(BitConverter.GetBytes(timeSent));
    }

    public static PlayerPacket Deserialize(byte[] data)
    {
        int currentIndex = 0;
        Vector3 position = NetworkExtensions.DeserializeVector3(data, ref currentIndex);
        Quaternion rotation = NetworkExtensions.DeserializeQuaternion(data, ref currentIndex);
        float timeSent = BitConverter.ToSingle(data, currentIndex);
        return new PlayerPacket(position, rotation,timeSent);
    }

}

