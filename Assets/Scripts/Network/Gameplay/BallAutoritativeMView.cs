using System;
using Byn.Net;
using UnityEngine;
using UnityEngine.Assertions;

class BallAutoritativeMView : ObservedComponent
{

    CustomRigidbody myRigidbody;

    [SerializeField]
    public GameObject ballModel;
    [SerializeField]
    public GameObject ballView;

    BallPacket? lastPacket;
    BallPacket currentPacket;

    Vector3 startPositionAtCurrentPacket;

    private bool uncatchableLastPacket = false;

    private float MAX_SPEED = 200;

    protected virtual void Awake()
    {
        myRigidbody = ballModel.GetComponent<CustomRigidbody>();
    }

    public override void OwnerUpdate()
    {
        ballView.transform.position = ballModel.transform.position;
    }

    public override void SimulationUpdate()
    {
        ballView.transform.position = Vector3.MoveTowards(ballView.transform.position, ballModel.transform.position, 0.5f);
    }

    protected override byte[] CreatePacket(long sendId)
    {
        return new BallPacket(sendId, myRigidbody.velocity, transform.position, MyComponents.TimeManagement.NetworkTimeInSeconds).Serialize();
    }

    public override void PacketReceived(ConnectionId id, byte[] data)
    {
        currentPacket = NetworkExtensions.Deserialize<BallPacket>(data);
        RewindAndReplay(currentPacket);
    }

    private void RewindAndReplay(BallPacket packet)
    {
        myRigidbody.velocity = packet.velocity;
        transform.position = packet.position;
        float time = packet.timeSent + Time.fixedDeltaTime;
        while (time < MyComponents.TimeManagement.NetworkTimeInSeconds - Time.fixedDeltaTime)
        {
            myRigidbody.Simulate(Time.fixedDeltaTime);
            time += Time.fixedDeltaTime;
        }
        myRigidbody.Simulate(MyComponents.TimeManagement.NetworkTimeInSeconds - time);
    }

    protected override bool IsSendingPackets()
    {
        return View.isMine;
    }

    public void Throw(Vector3 target, float power, float latencyinSeconds)
    {
        Vector3 velocity = new Vector3(target.x - ballModel.transform.position.x, 0, target.z - ballModel.transform.position.z);
        velocity.Normalize();
        myRigidbody.velocity = velocity * MAX_SPEED * Mathf.Max(power, 0.3f);
        //ballModel.transform.position = transform.position + myRigidbody.velocity * latencyinSeconds;
    }

    [Serializable]
    public struct BallPacket
    {
        public Vector3 velocity;
        public Vector3 position;
        public float timeSent;
        public long id;

        public BallPacket(long sendId, Vector3 velocity, Vector3 position, float time) : this()
        {
            this.id = sendId;
            this.velocity = velocity;
            this.position = position;
            this.timeSent = time;
        }
    }
}

