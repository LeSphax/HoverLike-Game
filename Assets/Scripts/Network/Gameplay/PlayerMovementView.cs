using Byn.Net;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public delegate void PacketHandler(byte[] data);

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementView : ObservedComponent
{

    Rigidbody myRigidbody;

    private float speed = 70;

    public static float MAX_VELOCITY = 45;
    private float ANGULAR_SPEED = 400;
    public Vector3? targetPosition;

    PacketHandler packetHandler;

    private int lostPackets = 0;
    private long receivedPackets
    {
        get
        {
            return lastReceivedId - firstPacketId;
        }
    }

    public float packetLossRatio
    {
        get
        {
            return (float)lostPackets / receivedPackets;
        }
    }

    private long firstPacketId = -1;
    private long lastReceivedId=-1;
    private long currentId
    {
        get
        {
            return lastReceivedId - 4;
        }
    }
    private const float FRAME_DURATION = 0.02f;

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
                return currentPacket;
            }
        }
    }

    protected virtual void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        packetHandler = ReceiveFirstData;
        PacketLoss.AddView(this);
    }

    protected override void OwnerUpdate()
    {
        if (targetPosition != null)
        {
            var lookPos = targetPosition.Value - transform.position;
            lookPos.y = 0;
            var targetRotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, FRAME_DURATION * ANGULAR_SPEED);
            myRigidbody.AddForce(transform.forward * speed * FRAME_DURATION, ForceMode.VelocityChange);

            ClampPlayerVelocity();
        }
    }

    protected override byte[] CreatePacket(long sendId)
    {
        PlayerPacket packet = new PlayerPacket();
        packet.velocity = myRigidbody.velocity;
        packet.position = transform.position;
        packet.rotation = transform.rotation;
        packet.id = sendId;
        return packet.Serialize();
    }

    public void ClampPlayerVelocity()
    {
        myRigidbody.velocity *= Mathf.Min(1.0f, MAX_VELOCITY / myRigidbody.velocity.magnitude);
    }

    protected override void SimulationUpdate()
    {
        while (StateBuffer.Count > 0 && currentId >= StateBuffer.Peek().id)
        {
            currentPacket = StateBuffer.Dequeue();
            //Debug.Log("Packet Consumed " + currentPacket.Value.id);
        }

        Assert.IsFalse((StateBuffer.Count == 0 || currentId >= StateBuffer.Peek().id) && currentPacket != null, "No Packet in buffer !!! " + currentId + "   " + gameObject.name);

        if (currentPacket != null)
        {
            double deltaTime = (nextPacket.Value.id - currentPacket.Value.id) * FRAME_DURATION;
            float completion = 0;
            if (deltaTime != 0)
                completion = (float)(FRAME_DURATION / deltaTime);
            transform.position = Vector3.Lerp(currentPacket.Value.position, nextPacket.Value.position, completion);
            transform.rotation = Quaternion.Lerp(currentPacket.Value.rotation, nextPacket.Value.rotation, completion);
        }
        else if (currentId >= 0)
        {
            Debug.LogWarning("No Packets for currentId " + currentId);
        }

    }


    public void ReceiveFirstData(byte[] data)
    {
        Debug.Log("ReceiveFirstData");
        PlayerPacket newPacket = NetworkExtensions.Deserialize<PlayerPacket>(data);
        StateBuffer.Enqueue(newPacket);
        firstPacketId = newPacket.id;
        lastReceivedId = firstPacketId;

        packetHandler = ReceiveData;
    }

    public void ReceiveData(byte[] data)
    {
        PlayerPacket newPacket = NetworkExtensions.Deserialize<PlayerPacket>(data);
        StateBuffer.Enqueue(newPacket);

        if (newPacket.id != lastReceivedId + 1)
        {
            lostPackets++;
        }
        lastReceivedId = newPacket.id;
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
    public double timeSent;
    public long id;
}

