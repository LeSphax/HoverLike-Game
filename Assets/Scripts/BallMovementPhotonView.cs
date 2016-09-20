using UnityEngine;

class BallMovementPhotonView : Photon.MonoBehaviour
{

    Rigidbody myRigidbody;
    PhotonView myPhotonView;
    double myLastSerializeTime;

    Vector3 myNetworkVelocity;
    Vector3 myNetworkPosition;

    public float speedDifference = 5;
    public float maxDistance = 5;

    private float MAX_SPEED = 200;
    private Color linesColor = Color.red;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        myPhotonView = gameObject.GetPhotonView();
    }

    void Update()
    {
        if (myPhotonView == null || myPhotonView.isMine == true || PhotonNetwork.connected == false)
        {
            return;
        }
        if (BallState.IsAttached())
        {
            myRigidbody.velocity = Vector3.zero;
            transform.localPosition = Vector3.one * 0.5f;
            return;
        }
        else
        {
            if (Vector3.Distance(myRigidbody.velocity, myNetworkVelocity) > myRigidbody.velocity.magnitude * 0.2f)
            {
                myRigidbody.velocity = myNetworkVelocity;
            }
            // myRigidbody.velocity = Vector3.Lerp(myRigidbody.velocity, myNetworkVelocity, Time.deltaTime * Vector3.Distance(myRigidbody.velocity, myNetworkVelocity) / (speedDifference - Vector3.Distance(myRigidbody.velocity, myNetworkVelocity)));
            transform.position = Vector3.Lerp(transform.position, myNetworkPosition, Time.deltaTime * Vector3.Distance(GetExtrapolatedPosition(), transform.position) / (maxDistance));
            if (Vector3.Distance(GetExtrapolatedPosition(), transform.position) > maxDistance * 10)
            {
                transform.position = GetExtrapolatedPosition();
            }
        }
        //transform.position = Vector3.Lerp(transform.position, GetExtrapolatedPosition(), Vector3.Distance(GetExtrapolatedPosition(), transform.position)/maxDistance);
    }

    public Vector3 GetExtrapolatedPosition()
    {
        float timePassed = (float)(PhotonNetwork.time - myLastSerializeTime);

        timePassed += (float)PhotonNetwork.GetPing() / 1000f;

        Vector3 extrapolatedPosition = myNetworkPosition + myNetworkVelocity * timePassed;

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
        DrawGizmos(extrapolatedPosition);
        return extrapolatedPosition;
    }

    private void DrawGizmos(Vector3 extrapolatedPosition)
    {
        if (linesColor == Color.red)
        {
            linesColor = Color.blue;
        }
        else
        {
            linesColor = Color.red;
        }
        Debug.DrawLine(transform.position, extrapolatedPosition, linesColor);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(myRigidbody.velocity);
            stream.SendNext(transform.position);
        }

        else if (stream.isReading)
        {
            myNetworkVelocity = (Vector3)stream.ReceiveNext();
            myNetworkPosition = (Vector3)stream.ReceiveNext();
            myLastSerializeTime = info.timestamp;
        }
    }

    public void Throw(Vector3 target, float power)
    {
        Vector3 velocity = new Vector3(target.x - transform.position.x, 0, target.z - transform.position.z);
        velocity.Normalize();
        myRigidbody.velocity += velocity * MAX_SPEED * Mathf.Max(power,0.3f);
        AttractionBall.activated = false;
        Invoke("ReactivateAttraction", 0.5f);
    }

    private void ReactivateAttraction()
    {
        AttractionBall.activated = true;
    }
}

