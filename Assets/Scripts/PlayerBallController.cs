using UnityEngine;

[RequireComponent(typeof(PowerBar))]
public class PlayerBallController : Photon.MonoBehaviour
{

    private Vector3 ballHoldingPosition;
    private PowerBar powerBar;

    public bool stealing = false;

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
        if (photonView.isMine)
        {
            if (Tags.IsPlayer(collision.gameObject.tag) && BallState.GetAttachedPlayerID() == collision.gameObject.GetPhotonView().viewID)
            {
                Debug.Log("Collision");
                if (stealing)
                    photonView.RPC("StealBall", PhotonTargets.MasterClient, BallState.GetAttachedPlayerID());
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (photonView.isMine)
        {
            if (collider.gameObject.tag == Tags.CatchDetector && !BallState.IsAttached())
            {
                photonView.RPC("PickUpBall", PhotonTargets.MasterClient);
            }
        }
    }

    [PunRPC]
    public void PickUpBall()
    {
        Debug.Log("PickUpBall " + gameObject.name);
        if (!BallState.IsAttached())
            BallState.SetAttached(photonView.viewID);
        photonView.RPC("AttachBall", PhotonTargets.All);
    }

    [PunRPC]
    public void StealBall(int victimId)
    {
        Debug.Log("StealBall " + victimId + "     " + photonView.viewID + "   " + gameObject.name);
        if (BallState.GetAttachedPlayerID() == victimId)
        {
            BallState.SetAttached(photonView.viewID);
            photonView.RPC("AttachBall", PhotonTargets.All);
        }
    }

    [PunRPC]
    public void AttachBall()
    {
        ball.transform.SetParent(transform);
        ball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        ball.transform.localPosition = ballHoldingPosition;
    }

    [PunRPC]
    void DetachBall()
    {
        Debug.Log("DetachBall " + gameObject.name);
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
        SetBallSpeed(target, power);
        //if (PhotonNetwork.isMasterClient)
        //{
        //    DetachBall();
        //    SetBallSpeed(target);
        //    photonView.RPC("DetachBall", PhotonTargets.Others);
        //}

    }

    private void SetBallSpeed(Vector3 target, float power)
    {
        ball.GetComponent<BallMovementPhotonView>().Throw(target, power);
    }

}
