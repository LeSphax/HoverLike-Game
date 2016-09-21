using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
class PlayerMovementPhotonView : Photon.MonoBehaviour
{

    Rigidbody myRigidbody;
    PhotonView myPhotonView;
    double myLastSerializeTime;

    Vector3 myNetworkVelocity;
    Vector3 myNetworkPosition;

    public float speedDifference = 5;
    public float maxDistance = 5;

    public float MAX_VELOCITY = 100f;
    public float ANGULAR_SPEED = 5.0f;
    private Color linesColor = Color.red;
    public GameObject target;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        myPhotonView = gameObject.GetPhotonView();
    }

    void Update()
    {
        if (target != null)
        {
            myRigidbody.drag = 2;
            var lookPos = target.transform.position - transform.position;
            lookPos.y = 0;
            var targetRotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * ANGULAR_SPEED);
            myRigidbody.AddForce(transform.forward * 2, ForceMode.VelocityChange);
        }
        else
        {
            myRigidbody.drag = 1;
        }
        GetComponent<Rigidbody>().velocity *= Mathf.Min(1.0f, MAX_VELOCITY / myRigidbody.velocity.magnitude);

        if (myPhotonView == null || myPhotonView.isMine == true || PhotonNetwork.connected == false)
        {
            return;
        }
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

    public Vector3 GetExtrapolatedPosition()
    {
        float timePassed = (float)(PhotonNetwork.time - myLastSerializeTime);

        timePassed += (float)PhotonNetwork.GetPing() / 1000f;

        Vector3 extrapolatedPosition = myNetworkPosition + myNetworkVelocity * timePassed;

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
}

