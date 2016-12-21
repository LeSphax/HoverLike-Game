using System;
using Byn.Net;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

class BallMovementView : ObservedComponent
{

    Rigidbody myRigidbody;

    private float MAX_SPEED = 150;

    PacketHandler packetHandler;

    private Queue<BallPacket> StateBuffer = new Queue<BallPacket>();
    private BallPacket? currentPacket = null;


    private BallPacket? nextPacket
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
        myRigidbody = GetComponent<Rigidbody>();
    }

    public void Throw(Vector3 target, float power)
    {
        Vector3 velocity = new Vector3(target.x - transform.position.x, 0, target.z - transform.position.z);
        velocity.Normalize();
        myRigidbody.velocity = velocity * MAX_SPEED * Mathf.Max(power, 0.4f);
        transform.position = transform.position;
    }

    public override void OwnerUpdate()
    {
        //DoNothing, the physics simulations are sufficient
    }

    public override void SimulationUpdate()
    {
        while (StateBuffer.Count > 0 && SimulationTime >= StateBuffer.Peek().timeSent)
        {
            currentPacket = StateBuffer.Dequeue();
        }

        if (MyComponents.BallState.IsAttached())
        {
            transform.localPosition = BallState.ballHoldingPosition;
            return;
        }
        else if (currentPacket != null)
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
        }
        else
        {
            transform.position = currentPacket.Value.position;
        }
    }

    protected override byte[] CreatePacket(long sendId)
    {
        return new BallPacket(transform.position, TimeManagement.NetworkTimeInSeconds).Serialize();
    }

    public override void PacketReceived(ConnectionId id, byte[] data)
    {
        BallPacket newPacket = BallPacket.Deserialize(data);
        StateBuffer.Enqueue(newPacket);
    }

    protected override bool IsSendingPackets()
    {
        return MyComponents.NetworkManagement.isServer;
    }

    [Serializable]
    public struct BallPacket
    {
        public Vector3 position;
        public float timeSent;

        public BallPacket(Vector3 position, float time) : this()
        {
            this.position = position;
            this.timeSent = time;
        }

        public byte[] Serialize()
        {
            byte[] data = NetworkExtensions.SerializeVector3(position);
            return ArrayExtensions.Concatenate(data, BitConverter.GetBytes(timeSent));
        }

        public static BallPacket Deserialize(byte[] data)
        {
            int currentIndex = 0;
            Vector3 position = NetworkExtensions.DeserializeVector3(data, ref currentIndex);
            float timeSent = BitConverter.ToSingle(data, currentIndex);
            return new BallPacket(position, timeSent);
        }
    }
}

