using Byn.Net;
using UnityEngine;

public class MovementInputManager : ObservedComponent
{

    public static float Acceleration = 100;

    protected Rigidbody myRigidbody;

    private Vector3 currentDirection;

    protected virtual void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    public override void OwnerUpdate()
    {
        SimulationUpdate();
    }

    public override void PacketReceived(ConnectionId id, byte[] data)
    {
        currentDirection = MovementInputPacket.Deserialize(data).GetDirection();
    }

    public override bool ShouldBatchPackets()
    {
        return false;
    }

    public override void SimulationUpdate()
    {
        if (MyComponents.NetworkManagement.IsServer)
        {
            //currentDirection = 
            
        }
    }

    protected override byte[] CreatePacket()
    {
        return new MovementInputPacket(
                Input.GetKey(KeyCode.W),
                Input.GetKey(KeyCode.S),
                Input.GetKey(KeyCode.A),
                Input.GetKey(KeyCode.D)
            ).Serialize();
    }

    protected override bool IsSendingPackets()
    {
        return true;
    }
}
