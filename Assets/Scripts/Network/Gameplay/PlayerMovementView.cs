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

    private float simulationStart;

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
                return null;
            }
        }
    }

    private float SimulationTime
    {
        get
        {
            return TimeManagement.NetworkTime - ClientDelay.Delay;
        }
    }

    protected virtual void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        packetHandler = ReceiveData;
    }

    public override void OwnerUpdate()
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
        packet.timeSent = TimeManagement.NetworkTime;
        return packet.Serialize();
    }

    public void ClampPlayerVelocity()
    {
        myRigidbody.velocity *= Mathf.Min(1.0f, MAX_VELOCITY / myRigidbody.velocity.magnitude);
    }

    public override void SimulationUpdate()
    {
        while (StateBuffer.Count > 0 && SimulationTime >= StateBuffer.Peek().timeSent)
        {
            currentPacket = StateBuffer.Dequeue();
            //Debug.Log("Packet Consumed " + currentPacket.Value.timeSent + "   " + SimulationTime);
        }

        Assert.IsFalse(StateBuffer.Count == 0 && currentPacket != null, "No Packet in buffer !!! " + SimulationTime + "   " + gameObject.name);

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

