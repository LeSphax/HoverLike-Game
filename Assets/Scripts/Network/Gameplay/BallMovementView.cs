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

    protected virtual void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    public void Throw(Vector3 target, float power)
    {
        Vector3 direction = new Vector3(target.x - transform.localPosition.x, 0, target.z - transform.localPosition.z);
        direction.Normalize();
        myRigidbody.velocity = direction * GetShootPowerLevel(power);
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
        while (StateBuffer.Count > 0 && TimeSimulation.TimeInSeconds > StateBuffer.GetFirst().simulationTime)
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
            float completion = (TimeSimulation.TimeInSeconds - currentPacket.simulationTime) / (NextPacket.simulationTime - currentPacket.simulationTime);
            transform.position = Vector3.Lerp(currentPacket.position, NextPacket.position, completion);
        }
        else
        {
            transform.position = currentPacket.position;
        }
    }


    BallPacket previousPacket;

    protected override byte[] CreatePacket()
    {
        BallPacket newPacket = new BallPacket(transform.position);
        if (previousPacket == null || previousPacket.Equals(newPacket))
        {
            previousPacket = newPacket;
            return null;
        }
        else
        {
            previousPacket = newPacket;
            return newPacket.Serialize();
        }
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
        return NetworkingState.IsServer;
    }

    public class BallPacket : IComparable
    {
        public Vector3 position;
        public float simulationTime;

        public BallPacket(Vector3 position)
        {
            this.position = position;
        }

        public BallPacket(Vector3 position, float simulationTime) : this(position)
        {
            this.simulationTime = simulationTime;
        }

        public byte[] Serialize()
        {
            return NetworkExtensions.SerializeVector3(position);
        }

        public static BallPacket Deserialize(byte[] data)
        {
            int currentIndex = 0;
            Vector3 position = NetworkExtensions.DeserializeVector3(data, ref currentIndex);
            return new BallPacket(position, ObservedComponent.LastBatchTime);
        }

        public int CompareTo(object obj)
        {
            Assert.IsTrue(obj is BallPacket);
            BallPacket other = (BallPacket)obj;
            float diff = simulationTime - other.simulationTime;
            return diff < 0 ? -1 : 1;
        }

        public bool Equals(BallPacket obj)
        {
            return position == obj.position;
        }
    }
}

