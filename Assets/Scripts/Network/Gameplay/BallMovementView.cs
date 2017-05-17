using System;
using Byn.Net;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using Wintellect.PowerCollections;

class BallMovementView : ObservedComponent
{

    Rigidbody myRigidbody;

    [SerializeField]
    private float MAX_SPEED;
    public static float ShootPowerLevel = 250;
    [SerializeField]
    private float MINIMUM_THROW_SPEED_PROPORTION;
    public static float MinimumShootPowerLevelProportion = 0.3f;

    PacketHandler packetHandler;

    private OrderedSet<BallPacket> StateBuffer = new OrderedSet<BallPacket>();
    private BallPacket currentPacket = null;


    private BallPacket nextPacket
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

    protected virtual void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        MAX_SPEED = ShootPowerLevel;
        MINIMUM_THROW_SPEED_PROPORTION = MinimumShootPowerLevelProportion;
    }

    public void Throw(Vector3 target, float power)
    {
        Vector3 velocity = new Vector3(target.x - transform.position.x, 0, target.z - transform.position.z);
        velocity.Normalize();
        myRigidbody.velocity = velocity * MAX_SPEED * Mathf.Max(power, MINIMUM_THROW_SPEED_PROPORTION);
        transform.position = transform.position;
    }

    public override void OwnerUpdate()
    {
        //DoNothing, the physics simulations are sufficient
    }

    public override void SimulationUpdate()
    {
        while (StateBuffer.Count > 0 && CurrentlyShownBatchNb > StateBuffer.GetFirst().batchNumber)
        {
            currentPacket = StateBuffer.GetFirst();
            currentPacket = StateBuffer.RemoveFirst();
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
            float completion = (CurrentlyShownBatchNb - currentPacket.batchNumber) / (nextPacket.batchNumber - currentPacket.batchNumber);
            transform.position = Vector3.Lerp(currentPacket.position, nextPacket.position, completion);
        }
        else
        {
            transform.position = currentPacket.position;
        }
    }

    protected override byte[] CreatePacket()
    {
        return new BallPacket(transform.position).Serialize();
    }

    public override bool ShouldBatchPackets()
    {
        return true;
    }

    public override void PacketReceived(ConnectionId id, byte[] data)
    {
        BallPacket newPacket = BallPacket.Deserialize(data);
        StateBuffer.Add(newPacket);
    }

    protected override bool IsSendingPackets()
    {
        return MyComponents.NetworkManagement.isServer;
    }

    public class BallPacket : IComparable
    {
        public Vector3 position;
        public int batchNumber;

        public BallPacket(Vector3 position)
        {
            this.position = position;
        }

        public BallPacket(Vector3 position, int batchNumber) : this(position)
        {
            this.batchNumber = batchNumber;
        }

        public byte[] Serialize()
        {
            return NetworkExtensions.SerializeVector3(position);
        }

        public static BallPacket Deserialize(byte[] data)
        {
            int currentIndex = 0;
            Vector3 position = NetworkExtensions.DeserializeVector3(data, ref currentIndex);
            return new BallPacket(position, ObservedComponent.LastReceivedBatchNumber);
        }

        public int CompareTo(object obj)
        {
            Assert.IsTrue(obj is BallPacket);
            BallPacket other = (BallPacket)obj;
            return batchNumber - other.batchNumber;
        }
    }
}

