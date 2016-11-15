using Byn.Net;
using PlayerManagement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public delegate void PacketHandler(byte[] data);

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerController))]
public class PlayerMovementView : ObservedComponent
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
    private PlayerController controller;

    private Rigidbody myRigidbody;

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
        set
        {
            strategy.targetPosition = value;
        }
    }



    protected virtual void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        controller = GetComponent<PlayerController>();
        if (!MyComponents.NetworkManagement.isServer)
            myRigidbody.isKinematic = true;
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
        PlayerPacket packet = new PlayerPacket();
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

    public override void PacketReceived(ConnectionId id, byte[] data)
    {
        PlayerPacket newPacket = NetworkExtensions.Deserialize<PlayerPacket>(data);
        StateBuffer.Enqueue(newPacket);
    }

    protected override bool IsSendingPackets()
    {
        return MyComponents.NetworkManagement.isServer;
    }
}

[Serializable]
public struct PlayerPacket
{
    public Vector3 position;
    public Quaternion rotation;
    public float timeSent;
    //public long id;
}

