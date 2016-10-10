using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(PowerBar))]
public class PlayerBallController : SlideBall.MonoBehaviour
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
        MyGameObjects.Properties.AddListener(PropertiesKeys.NamePlayerHoldingBall, AttachBall);
        Physics.IgnoreCollision(GetComponent<Collider>(), ball.GetComponent<Collider>(), true);
        powerBar = GetComponent<PowerBar>();
        ballHoldingPosition = new Vector3(.5f, .5f, .5f);

    }

    void OnCollisionEnter(Collision collision)
    {
        if (View.isMine)
        {
            if (Tags.IsPlayer(collision.gameObject.tag) && BallState.GetAttachedPlayerID() == collision.gameObject.GetNetworkView().viewId)
            {
                Debug.Log("Collision");
                if (stealing)
                    View.RPC("StealBall", RPCTargets.Server, BallState.GetAttachedPlayerID());
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (View.isMine)
        {
            if (collider.gameObject.tag == Tags.CatchDetector && !BallState.IsAttached())
            {
                View.RPC("PickUpBall", RPCTargets.Server);
            }
        }
    }

    [MyRPC]
    public void PickUpBall()
    {
        Debug.Log("PickUpBall " + gameObject.name + "   " + BallState.IsAttached());
        Assert.IsTrue(MyGameObjects.NetworkManagement.isServer);
        if (!BallState.IsAttached())
            BallState.SetAttached(View.viewId);
    }

    [MyRPC]
    private void StealBall(int victimId)
    {
        Debug.Log("StealBall " + victimId + "     " + View.viewId + "   " + gameObject.name);
        Assert.IsTrue(MyGameObjects.NetworkManagement.isServer);
        if (BallState.GetAttachedPlayerID() == victimId)
        {
            BallState.SetAttached(View.viewId);
        }
    }

    public void AttachBall(object previousPlayer, object newPlayer)
    {
        int newPlayerId = newPlayer == null ? 0 : (int)newPlayer;
        int previousPlayerId = previousPlayer == null ? 0 : (int)previousPlayer;
        bool attach = newPlayerId == View.viewId && previousPlayerId != View.viewId;
        bool detach = previousPlayerId == View.viewId && newPlayerId != View.viewId;
        if (attach)
        {
            AttachBall(true);
        }
        else if (detach)
        {
            AttachBall(false);
        }
    }

    public void AttachBall(bool attach)
    {
        if (attach)
        {
            ball.transform.SetParent(transform);
            ball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            ball.transform.localPosition = ballHoldingPosition;
        }
        else
        {
            Physics.IgnoreCollision(ball.GetComponent<Collider>(), GetComponent<Collider>());
            ball.transform.SetParent(null);
            ball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }

    void Update()
    {
        if (View.isMine)
        {
            UpdateThrow();
        }
    }

    private void UpdateThrow()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (BallState.GetAttachedPlayerID() == View.viewId)
            {
                powerBar.StartFilling();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (BallState.GetAttachedPlayerID() == View.viewId)
            {
                ClientThrowBall(Functions.GetMouseWorldPosition(), powerBar.powerValue);
                powerBar.Hide();
            }
        }
    }

    private void ClientThrowBall(Vector3 target, float power)
    {
        AttachBall(false);
        SetBallSpeed(target, power);
        BallState.ListenToServer = false;
        View.RPC("ServerThrowBall", RPCTargets.Server, View.viewId, target, power);
    }

    [MyRPC]
    private void ServerThrowBall(int throwerId, Vector3 target, float power)
    {
        Debug.Log("We should extrapolate the position of the ball considering the time needed for the packet to arrive");
        //Check if ClientThrowBall was already called to avoid setting ball speed twice
        if (BallState.ListenToServer)
            SetBallSpeed(target, power);
        if (throwerId == BallState.GetAttachedPlayerID())
            BallState.SetAttached(-1);
    }


    private void SetBallSpeed(Vector3 target, float power)
    {
        ball.GetComponent<BallMovementPhotonView>().Throw(target, power);
    }

}
