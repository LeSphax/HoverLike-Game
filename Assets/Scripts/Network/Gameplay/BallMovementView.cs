using System;
using Byn.Net;
using UnityEngine;

class BallMovementView : ObservedComponent
{

    Rigidbody myRigidbody;

    BallPacket currentPacket;

    public float speedDifference = 5;
    public float maxDistance = 5;

    private float MAX_SPEED = 200;

    protected virtual void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    public Vector3 GetExtrapolatedPosition()
    {
        float timePassed = (float)(TimeManagement.NetworkTime - currentPacket.timeSent);

        Vector3 extrapolatedPosition = currentPacket.position + currentPacket.velocity * timePassed;

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

    public void Throw(Vector3 target, float power)
    {
        Vector3 velocity = new Vector3(target.x - transform.position.x, 0, target.z - transform.position.z);
        velocity.Normalize();
        myRigidbody.velocity = velocity * MAX_SPEED * Mathf.Max(power, 0.3f);
        Debug.Log("Need to check the functionnement of AttractionBall");
    }

    public override void OwnerUpdate()
    {
        //DoNothing, the physics simulations are sufficient
    }

    public override void SimulationUpdate()
    {
        //Debug.Log(currentPacket.position + "   " + BallState.ListenToServer + "   " + BallState.IsAttached());
        if (BallState.ListenToServer)
        {
            if (BallState.IsAttached())
            {
                myRigidbody.velocity = Vector3.zero;
                transform.localPosition = Vector3.one * 0.5f;
                return;
            }
            else
            {
                if (Vector3.Distance(myRigidbody.velocity, currentPacket.velocity) > myRigidbody.velocity.magnitude * 0.2f)
                {
                    myRigidbody.velocity = currentPacket.velocity;
                }
                // myRigidbody.velocity = Vector3.Lerp(myRigidbody.velocity, myNetworkVelocity, Time.deltaTime * Vector3.Distance(myRigidbody.velocity, myNetworkVelocity) / (speedDifference - Vector3.Distance(myRigidbody.velocity, myNetworkVelocity)));
                transform.position = Vector3.Lerp(transform.position, currentPacket.position, Time.deltaTime * Vector3.Distance(GetExtrapolatedPosition(), transform.position) / (maxDistance));
                if (Vector3.Distance(GetExtrapolatedPosition(), transform.position) > maxDistance * 10)
                {
                    transform.position = GetExtrapolatedPosition();
                }
            }
            //transform.position = Vector3.Lerp(transform.position, GetExtrapolatedPosition(), Vector3.Distance(GetExtrapolatedPosition(), transform.position)/maxDistance);
        }
    }

    protected override byte[] CreatePacket(long sendId)
    {
        return new BallPacket(sendId, myRigidbody.velocity, transform.position, TimeManagement.NetworkTime).Serialize();
    }

    public override void PacketReceived(ConnectionId id, byte[] data)
    {
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

