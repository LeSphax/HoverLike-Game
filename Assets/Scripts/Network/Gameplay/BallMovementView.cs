using System;
using Byn.Net;
using UnityEngine;
using UnityEngine.Assertions;

class BallMovementView : ObservedComponent
{

    CustomRigidbody myRigidbody;

    BallPacket? lastPacket;
    BallPacket currentPacket;

    Vector3 startPositionAtCurrentPacket;

    private bool uncatchableLastPacket = false;

    public float speedDifference = 5;
    public float maxDistance = 5;

    private float MAX_SPEED = 200;

    protected virtual void Awake()
    {
        myRigidbody = GetComponent<CustomRigidbody>();
    }

    public Vector3 GetExtrapolatedPosition()
    {
        float timePassed = MyComponents.TimeManagement.NetworkTimeInSeconds - currentPacket.timeSent;

        Vector3 extrapolatedPosition;
        extrapolatedPosition = currentPacket.position + currentPacket.velocity * timePassed;

        //RaycastHit hit;

        //if (Physics.Raycast(myNetworkPosition, extrapolatedPosition, out hit, Vector3.Distance(myNetworkPosition,extrapolatedPosition)))
        //{
        //    if (hit.collider.gameObject.tag != Tags.Player && Vector3.Distance(transform.position, hit.point) < 10)
        //    {
        //        myNetworkPosition = hit.point;
        //        myNetworkVelocity = Vector3.Reflect(myNetworkVelocity, hit.normal);
        //        myLastSerializeTime = PhotonNetwork.time;
        //        //extrapolatedPosition = GetExtrapolatedPosition();
        //    }
        //}
        return extrapolatedPosition;
    }

    public void SetPositionFromExtrapolation()
    {
        Assert.IsTrue(MyComponents.BallState.Uncatchable);
        if (lastPacket != null)
        {
            float timePassed = MyComponents.TimeManagement.NetworkTimeInSeconds - currentPacket.timeSent;
            float timeBetweenPackets = currentPacket.timeSent - lastPacket.Value.timeSent;
            Vector3 extrapolatedPosition = currentPacket.position + currentPacket.position - lastPacket.Value.position;
            transform.position = Vector3.Lerp(startPositionAtCurrentPacket, extrapolatedPosition, timePassed / timeBetweenPackets);
        }
        else
        {
            transform.position = currentPacket.position;
        }
    }
    public void Throw(Vector3 target, float power, float latencyinSeconds)
    {
        Vector3 velocity = new Vector3(target.x - transform.position.x, 0, target.z - transform.position.z);
        velocity.Normalize();
        myRigidbody.velocity = velocity * MAX_SPEED * Mathf.Max(power, 0.3f);
        transform.position = transform.position + myRigidbody.velocity * latencyinSeconds;
    }

    public override void OwnerUpdate()
    {
        //DoNothing, the physics simulations are sufficient
    }

    public override void SimulationUpdate()
    {
        //Debug.Log(currentPacket.position + "   " + BallState.ListenToServer + "   " + BallState.IsAttached());
        //if (MyComponents.BallState.ListenToServer)
        if (MyComponents.BallState.IsAttached())
        {
            myRigidbody.velocity = Vector3.zero;
            transform.localPosition = BallState.ballHoldingPosition;
            return;
        }
        else if (MyComponents.BallState.Uncatchable)
        {
            SetPositionFromExtrapolation();
        }
        else
        {
            if (Vector3.Distance(myRigidbody.velocity, currentPacket.velocity) > myRigidbody.velocity.magnitude * 0.2f)
            {
                myRigidbody.velocity = currentPacket.velocity;
            }
            //myRigidbody.velocity = Vector3.Lerp(myRigidbody.velocity, myNetworkVelocity, Time.deltaTime * Vector3.Distance(myRigidbody.velocity, myNetworkVelocity) / (speedDifference - Vector3.Distance(myRigidbody.velocity, myNetworkVelocity)));
            transform.position = Vector3.Lerp(transform.position, currentPacket.position, Time.deltaTime * Vector3.Distance(GetExtrapolatedPosition(), transform.position) / (maxDistance));
            if (Vector3.Distance(GetExtrapolatedPosition(), transform.position) > maxDistance)
            {
                transform.position = GetExtrapolatedPosition();
            }
        }
        //transform.position = Vector3.Lerp(transform.position, GetExtrapolatedPosition(), Vector3.Distance(GetExtrapolatedPosition(), transform.position)/maxDistance);
    }

    protected override byte[] CreatePacket(long sendId)
    {
        return new BallPacket(sendId, myRigidbody.velocity, transform.position, MyComponents.TimeManagement.NetworkTimeInSeconds).Serialize();
    }

    public override void PacketReceived(ConnectionId id, byte[] data)
    {
        if (MyComponents.BallState.Uncatchable)
        {
            if (uncatchableLastPacket)
            {
                startPositionAtCurrentPacket = transform.position;
                lastPacket = currentPacket;
            }
            uncatchableLastPacket = true;
        }
        else
        {
            uncatchableLastPacket = false;
            lastPacket = null;
        }
        currentPacket = NetworkExtensions.Deserialize<BallPacket>(data);
    }

    protected override bool IsSendingPackets()
    {
        return View.isMine;
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

