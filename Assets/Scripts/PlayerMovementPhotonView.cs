using System;
using UnityEngine;

[RequireComponent(typeof(CustomRigidbody))]
class PlayerMovementPhotonView : Photon.MonoBehaviour
{

    CustomRigidbody myRigidbody;
    PhotonView myPhotonView;
    double myLastSerializeTime;

    public float speedDifference = 5;
    public float maxDistance = 5;

    public float MAX_VELOCITY = 100f;
    public float ANGULAR_SPEED = 5.0f;
    private Color linesColor = Color.red;
    public Vector3? targetPosition;
    public float timeLastUpdate;

    private const float FRAME_DURATION = 0.02f;

    private int currentId = 0;

    private PlayerPacket[] StateBuffer;

    void Awake()
    {
        myRigidbody = GetComponent<CustomRigidbody>();
        myPhotonView = gameObject.GetPhotonView();
    }

    void Start()
    {
        InvokeRepeating("CustomUpdate", 0f, FRAME_DURATION);
    }

    void CustomUpdate()
    {
        if (targetPosition != null)
        {
            myRigidbody.drag = 3;
            var lookPos = targetPosition.Value - transform.position;
            lookPos.y = 0;
            var targetRotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * ANGULAR_SPEED);
            myRigidbody.AddForce(transform.forward * 2);
        }
        else
        {
            myRigidbody.drag = 1;
        }
        GetComponent<CustomRigidbody>().velocity *= Mathf.Min(1.0f, MAX_VELOCITY / myRigidbody.velocity.magnitude);
        myRigidbody.CustomUpdate();
        timeLastUpdate = Time.realtimeSinceStartup;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(myRigidbody.velocity);
            stream.SendNext(transform.position);
            stream.SendNext(targetPosition);
            stream.SendNext(timeLastUpdate);
        }

        else if (stream.isReading)
        {
            StateBuffer[currentId] = new PlayerPacket();
            StateBuffer[currentId].id = currentId;
            StateBuffer[currentId].myNetworkVelocity = (Vector3)stream.ReceiveNext();
            StateBuffer[currentId].myNetworkPosition = (Vector3)stream.ReceiveNext();
            StateBuffer[currentId].myNetworkTarget = (Vector3?)stream.ReceiveNext();
            StateBuffer[currentId].myNetworkLastUpdate = (float)stream.ReceiveNext();
            myLastSerializeTime = info.timestamp;
            RefreshSimulation();
        }
    }

    private void RefreshSimulation()
    {
        transform.position = myNetworkPosition;
        myRigidbody.velocity = myNetworkVelocity;
        targetPosition = myNetworkTarget;
        float timePassed = PhotonNetwork.GetPing() / 1000f;
        Simulate(timePassed);
    }


    /*
     * Simulate the passaing of time seconds
     */
    public void Simulate(float time)
    {
        int numberFrames = (int)(time / FRAME_DURATION);
        float residualTime = time % FRAME_DURATION;
        for (int i = 0; i < numberFrames; i++)
        {
            CustomUpdate();
        }
        CancelInvoke("CustomUpdate");
        InvokeRepeating("CustomUpdate", residualTime, FRAME_DURATION);
    }
}

public struct PlayerPacket
{
    public int id;
    public Vector3 myNetworkVelocity;
    public Vector3 myNetworkPosition;
    public Vector3? myNetworkTarget;
    public float myNetworkLastUpdate;


}

