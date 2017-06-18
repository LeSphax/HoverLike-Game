using Ball;
using Byn.Net;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using Wintellect.PowerCollections;

class BallMovementView : ObservedComponent
{

    Rigidbody myRigidbody;

    public static float ShootPowerLevel = 220;

    public static float MinimumShootPowerLevelProportion = 0.3f;

    PacketHandler packetHandler;

    private OrderedSet<BallPacket> StateBuffer = new OrderedSet<BallPacket>();
    private BallPacket currentPacket = null;


    private BallPacket NextPacket
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
    }

    public void Throw(Vector3 target, float power)
    {
        Vector3 direction = new Vector3(target.x - transform.position.x, 0, target.z - transform.position.z);
        direction.Normalize();
        myRigidbody.velocity = direction * GetShootPowerLevel(power);
        transform.position = transform.position;
    }

    public static float GetShootPowerLevel(float power)
    {
        return ShootPowerLevel * (power + MinimumShootPowerLevelProportion);
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
            transform.localPosition = AttachedTrajectoryStrategy.ballHoldingPosition;
            return;
        }
        else if (currentPacket != null)
        {
            InterpolateMovement();
        }
    }

    private void InterpolateMovement()
    {
        if (NextPacket != null)
        {
            float completion = (CurrentlyShownBatchNb - currentPacket.batchNumber) / (NextPacket.batchNumber - currentPacket.batchNumber);
            transform.position = Vector3.Lerp(currentPacket.position, NextPacket.position, completion);
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
        return MyComponents.NetworkManagement.IsServer;
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

