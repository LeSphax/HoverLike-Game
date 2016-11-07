using Byn.Net;
using PlayerManagement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public delegate void PacketHandler(byte[] data);

[RequireComponent(typeof(CustomRigidbody))]
[RequireComponent(typeof(PlayerController))]
public class PlayerMovementView : ObservedComponent
{
    private ConnectionId? playerConnectionId = null;
    private Player Player
    {
        get
        {
            Assert.IsTrue(playerConnectionId != null);
            return PlayerView.GetMyPlayer(View.isMine, playerConnectionId.Value);
        }
    }

    private PlayerMovementStrategy strategy;

    private CustomRigidbody myRigidbody;

    PacketHandler packetHandler;

    private Queue<PlayerPacket> StateBuffer = new Queue<PlayerPacket>();
    private PlayerPacket? currentPacket = null;
    internal Vector3? targetPosition
    {
        set
        {
            strategy.targetPosition = value;
        }
    }

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

    protected virtual void Awake()
    {
        myRigidbody = GetComponent<CustomRigidbody>();
        packetHandler = ReceiveData;
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
        strategy.UpdateMovement();
    }

    protected override byte[] CreatePacket(long sendId)
    {
        PlayerPacket packet = new PlayerPacket();
        packet.velocity = myRigidbody.velocity;
        packet.position = transform.position;
        packet.rotation = transform.rotation;
        packet.timeSent = TimeManagement.NetworkTimeInSeconds;
        return packet.Serialize();
    }

    public override void SimulationUpdate()
    {
        while (StateBuffer.Count > 0 && SimulationTime >= StateBuffer.Peek().timeSent)
        {
            currentPacket = StateBuffer.Dequeue();
            //Debug.Log("Packet Consumed " + currentPacket.Value.timeSent + "   " + SimulationTime);
        }

        //if (StateBuffer.Count == 0 && currentPacket != null)
        //Debug.LogWarning("No Packet in buffer !!! " + SimulationTime + "   " + gameObject.name);

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

    public void ReceiveData(byte[] data)
    {
        PlayerPacket newPacket = NetworkExtensions.Deserialize<PlayerPacket>(data);
        StateBuffer.Enqueue(newPacket);
    }

    public override void PacketReceived(ConnectionId id, byte[] data)
    {
        packetHandler.Invoke(data);
    }

    protected override bool IsSendingPackets()
    {
        return View.isMine;
    }
}

[Serializable]
public struct PlayerPacket
{
    public Vector3 velocity;
    public Vector3 position;
    public Quaternion rotation;
    public float timeSent;
    //public long id;
}

