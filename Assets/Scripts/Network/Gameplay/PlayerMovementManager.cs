using Byn.Net;
using PlayerManagement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Wintellect.PowerCollections;

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

    private OrderedSet<PlayerPacket> StateBuffer = new OrderedSet<PlayerPacket>();
    private PlayerPacket currentPacket = null;


    private PlayerPacket nextPacket
    {
        get
        {
            if (StateBuffer.Count > 0)
            {
                return StateBuffer.GetFirst();
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

    protected override byte[] CreatePacket()
    {
        return new PlayerPacket(transform.position, transform.rotation).Serialize();
    }

    public override bool ShouldBatchPackets()
    {
        return true;
    }

    public override void SimulationUpdate()
    {
        //Debug.Log(CurrentlyShownBatchNb + "     " + LastReceivedBatchNumber + "     " + StateBuffer.GetFirst().batchNumber);

        while (StateBuffer.Count > 0 && CurrentlyShownBatchNb > StateBuffer.GetFirst().batchNumber)
        {
            currentPacket = StateBuffer.GetFirst();
            currentPacket = StateBuffer.RemoveFirst();
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
            float completion = (CurrentlyShownBatchNb -currentPacket.batchNumber)/ (nextPacket.batchNumber - currentPacket.batchNumber);
            transform.position = Vector3.Lerp(currentPacket.position, nextPacket.position, completion);
            transform.rotation = Quaternion.Lerp(currentPacket.rotation, nextPacket.rotation, completion);
            //if (Player.IsMyPlayer)
            //    Debug.Log("Deplacement " + (transform.position - previousPosition) + "    " + completion + "   " + (nextPacket.position - currentPacket.position));
        }
        else
        {
            transform.position = currentPacket.position;
            transform.rotation = currentPacket.rotation;
        }
    }

    public override void PacketReceived(ConnectionId id, byte[] data)
    {
        PlayerPacket newPacket = PlayerPacket.Deserialize(data);
        StateBuffer.Add(newPacket);
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

public class PlayerPacket : IComparable
{
    public Vector3 position;
    public Quaternion rotation;
    public int batchNumber;

    public PlayerPacket(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }

    public PlayerPacket(Vector3 position, Quaternion rotation, int batchNumber) : this(position, rotation)
    {
        this.batchNumber = batchNumber;
    }

    public byte[] Serialize()
    {
        byte[] data = NetworkExtensions.SerializeVector3(position);
        return ArrayExtensions.Concatenate(data, NetworkExtensions.SerializeQuaternion(rotation));
    }

    public static PlayerPacket Deserialize(byte[] data)
    {
        int currentIndex = 0;
        Vector3 position = NetworkExtensions.DeserializeVector3(data, ref currentIndex);
        Quaternion rotation = NetworkExtensions.DeserializeQuaternion(data, ref currentIndex);
        return new PlayerPacket(position, rotation, ObservedComponent.LastReceivedBatchNumber);
    }

    public int CompareTo(object obj)
    {
        Assert.IsTrue(obj is PlayerPacket);
        PlayerPacket other = (PlayerPacket)obj;
        return batchNumber - other.batchNumber;
    }
}

