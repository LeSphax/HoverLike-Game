using Byn.Net;
using PlayerManagement;
using System;
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

    public float MaxPlayerVelocity
    {
        get
        {
            return strategy.MaxPlayerVelocity;
        }
    }

    private PlayerMovementStrategy strategy;
    internal PlayerController controller;

    private OrderedSet<PlayerPacket> StateBuffer = new OrderedSet<PlayerPacket>();
    private PlayerPacket currentPacket = null;


    private PlayerPacket NextPacket
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

    internal Vector3? TargetPosition
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
        if (Player.IsMyPlayer)
        {
            MyComponents.BattleriteCamera.PositionCamera();
        }
    }

    PlayerPacket previousPacket;

    protected override byte[] CreatePacket()
    {
        PlayerPacket newPacket = new PlayerPacket(transform.position, transform.rotation);
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

    public override void SimulationUpdate()
    {
        while (StateBuffer.Count > 0 && TimeSimulation.TimeInSeconds > StateBuffer.GetFirst().simulationTime)
        {
            Debug.Log("Dismissing 1 packet");
            currentPacket = StateBuffer.GetFirst();
            currentPacket = StateBuffer.RemoveFirst();
        }

        if (currentPacket != null)
        {
            InterpolateMovement();
            if (Player.IsMyPlayer)
            {
                MyComponents.BattleriteCamera.PositionCamera();
            }
        }
    }

    Vector3 prevPos;
    PlayerPacket prevPacket = new PlayerPacket(Vector3.zero,Quaternion.identity);

    private void InterpolateMovement()
    {
        if (NextPacket != null)
        {
            float completion = (TimeSimulation.TimeInSeconds - currentPacket.simulationTime) / (NextPacket.simulationTime - currentPacket.simulationTime);
            transform.position = Vector3.Lerp(currentPacket.position, NextPacket.position, completion);
            transform.rotation = Quaternion.Lerp(currentPacket.rotation, NextPacket.rotation, completion);
            if (Player.IsMyPlayer)
            {
                //Debug.Log("Player : " + (transform.position - prevPos).magnitude * Time.deltaTime + "    " + (transform.position - prevPos));
                string nPacketValues = NextPacket != null ? NextPacket.position + ", " + NextPacket.simulationTime : null;
                string cPacketValues = currentPacket != null ? currentPacket.position + ", " + currentPacket.simulationTime : null;
                //Debug.Log("Player : " + cPacketValues + "   " + nPacketValues + "   "  + completion + "    " + Time.deltaTime + "   " + (transform.position - prevPos));

                Debug.Log("Packets : " + cPacketValues + "   " + nPacketValues + "   " + TimeSimulation.TimeInSeconds);
                Debug.Log("Player : " + ((NextPacket.position - currentPacket.position) * (NextPacket.simulationTime - currentPacket.simulationTime)).magnitude
                    + "   "  + completion + "    " + Time.deltaTime + "   " + (transform.position - prevPos));
                Debug.Log("Player2 " + ((NextPacket.position - prevPacket.position) * (NextPacket.simulationTime - prevPacket.simulationTime)).magnitude
                    + "   " + (TimeSimulation.TimeInSeconds - prevPacket.simulationTime) / (NextPacket.simulationTime - prevPacket.simulationTime));
                prevPacket = currentPacket;
                prevPos = transform.position;
                if(completion >= 1)
                {
                    Debug.LogError("Completion too high");
                }
            }
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
        return MyComponents.NetworkManagement.IsServer;
    }

    public void Brake(bool activate)
    {
        Assert.IsTrue(strategy.GetType() == typeof(AttackerMovementStrategy));
        ((AttackerMovementStrategy)strategy).Brake(activate);
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
    public float simulationTime;

    public PlayerPacket(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }

    public PlayerPacket(Vector3 position, Quaternion rotation, float simulationTime) : this(position, rotation)
    {
        this.simulationTime = simulationTime;
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
        return new PlayerPacket(position, rotation, ObservedComponent.LastBatchTime);
    }

    public int CompareTo(object obj)
    {
        Assert.IsTrue(obj is PlayerPacket);
        PlayerPacket other = (PlayerPacket)obj;
        float diff = simulationTime - other.simulationTime;
        return diff < 0 ? -1 : 1;
    }

    public bool Equals(PlayerPacket obj)
    {
        return position == obj.position && rotation == obj.rotation;
    }
}


