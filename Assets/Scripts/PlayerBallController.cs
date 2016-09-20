using UnityEngine;

public class PlayerBallController : Photon.MonoBehaviour
{

    private Vector3 ballHoldingPosition;
    public PowerBar powerBar;

    private GameObject _ball;
    private GameObject ball
    {
        get
        {
            if (_ball == null)
            {
                _ball = GameObject.FindGameObjectWithTag(Tags.Ball);
            }
            return _ball;
        }
    }


    void Start()
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), ball.GetComponent<Collider>(), true);
        powerBar = GetComponent<PowerBar>();
        ballHoldingPosition = new Vector3(.5f, .5f, .5f);

    }

    void OnCollisionEnter(Collision collision)
    {
        if (ball.GetPhotonView().isMine)
        {
            if (collision.gameObject.tag == Tags.Player && BallState.GetAttachedPlayerID() != photonView.viewID && BallState.GetAttachedPlayerID() == collision.gameObject.GetPhotonView().viewID)
            {
                Debug.Log("PlayerEnter");
                if (BallState.IsTakeable())
                {
                    AttachBall();
                    photonView.RPC("AttachBall", PhotonTargets.Others);
                }
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (ball.GetPhotonView().isMine)
        {
            if (collider.gameObject.tag == Tags.CatchDetector && BallState.GetAttachedPlayerID() != photonView.viewID)
            {
                if (BallState.IsCatchDetectorEnabled())
                {
                    AttachBall();
                    photonView.RPC("AttachBall", PhotonTargets.Others);
                }
            }
        }
    }

    [PunRPC]
    public void AttachBall()
    {
        Debug.Log("Attach");
        if (ball.GetPhotonView().isMine)
            BallState.SetAttached(photonView.viewID);
        ball.transform.SetParent(transform);
        ball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        ball.transform.localPosition = ballHoldingPosition;
    }

    [PunRPC]
    void DetachBall()
    {
        Debug.Log("Detach");
        Physics.IgnoreCollision(ball.GetComponent<Collider>(), GetComponent<Collider>());
        BallState.Detach();
        ball.transform.SetParent(null);
        ball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }

    void Update()
    {
        if (photonView.isMine)
        {
            UpdateThrow();
        }
    }

    private void UpdateThrow()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (BallState.GetAttachedPlayerID() == photonView.viewID)
            {
                powerBar.StartFilling();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (BallState.GetAttachedPlayerID() == photonView.viewID)
            {
                photonView.RPC("ThrowBall", PhotonTargets.AllViaServer, Functions.GetMouseWorldPosition(), powerBar.powerValue);
                powerBar.Hide();
            }
        }
    }

    [PunRPC]
    private void ThrowBall(Vector3 target, float power)
    {
        DetachBall();
        SetBallSpeed(target,power);
        //if (PhotonNetwork.isMasterClient)
        //{
        //    DetachBall();
        //    SetBallSpeed(target);
        //    photonView.RPC("DetachBall", PhotonTargets.Others);
        //}

    }

    private void SetBallSpeed(Vector3 target,float power)
    {
        ball.GetComponent<BallMovementPhotonView>().Throw(target, power);
    }

    [PunRPC]
    public void RequestControlOfTheBall()
    {
        if (photonView.isMine)
        {
            Debug.Log("RequestOwnership");
            ball.GetPhotonView().RequestOwnership();
        }
        Debug.Log(ball.GetPhotonView().isMine);
    }

}
