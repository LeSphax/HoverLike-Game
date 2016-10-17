using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(PowerBar))]
public class PlayerBallController : SlideBall.MonoBehaviour
{

    private PowerBar powerBar;
    private bool tryingToCatchBall = true;

    public bool stealing = false;

    private GameObject ball
    {
        get
        {
            return BallState.ball;
        }
    }


    protected void Start()
    {
        MyGameObjects.Properties.AddListener(PropertiesKeys.NamePlayerHoldingBall, AttachBall);
        Physics.IgnoreCollision(GetComponent<Collider>(), ball.GetComponent<Collider>(), true);
        powerBar = GetComponent<PowerBar>();

    }

    void OnCollisionEnter(Collision collision)
    {
        if (View.isMine)
        {
            if (Tags.IsPlayer(collision.gameObject.tag) && BallState.GetAttachedPlayerID() == collision.gameObject.GetNetworkView().ViewId)
            {
                Debug.Log("Collision");
                if (stealing)
                    View.RPC("StealBall", RPCTargets.Server, BallState.GetAttachedPlayerID());
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (View.isMine)
        {
            if (collider.gameObject.tag == Tags.CatchDetector)
            {
                Debug.LogError("Exit" + collider);
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (View.isMine)
        {
            if (collider.gameObject.tag == Tags.CatchDetector && !BallState.IsAttached() && tryingToCatchBall)
            {
                Debug.LogError("Enter" + collider);
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
        {
            BallState.SetAttached(View.ViewId);
        }
    }

    [MyRPC]
    private void StealBall(int victimId)
    {
        Debug.Log("StealBall " + victimId + "     " + View.ViewId + "   " + gameObject.name);
        Assert.IsTrue(MyGameObjects.NetworkManagement.isServer);
        if (BallState.GetAttachedPlayerID() == victimId)
        {
            Debug.LogWarning("SetAttached " + View.ViewId);
            BallState.SetAttached(View.ViewId);
        }
    }

    public void AttachBall(object previousPlayer, object newPlayer)
    {
        int newPlayerId = newPlayer == null ? -1 : (int)newPlayer;
        int previousPlayerId = previousPlayer == null ? -1 : (int)previousPlayer;

        bool attach = newPlayerId == View.ViewId && previousPlayerId != View.ViewId;
        bool detach = previousPlayerId == View.ViewId && newPlayerId != View.ViewId;

        Assert.IsFalse(attach && detach);

        if (attach)
        {
            BallState.AttachBall(View.ViewId);
        }
        else if (detach)
        {
            BallState.AttachBall(-1);
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
            if (BallState.GetAttachedPlayerID() == View.ViewId)
            {
                powerBar.StartFilling();
            }
        }
        else if (Input.GetMouseButtonUp(0) && powerBar.IsFilling())
        {
            if (BallState.GetAttachedPlayerID() == View.ViewId)
            {
                ClientThrowBall(Functions.GetMouseWorldPosition(), powerBar.powerValue);
                powerBar.Hide();
            }
        }
        else if (Input.GetMouseButtonDown(1) && powerBar.IsFilling())
        {
            powerBar.Hide();
        }
    }

    private void ClientThrowBall(Vector3 target, float power)
    {
        BallState.AttachBall(-1);
        SetBallSpeed(target, power);
        BallState.ListenToServer = false;
        View.RPC("ServerThrowBall", RPCTargets.Server, View.ViewId, target, power);
    }

    [MyRPC]
    private void ServerThrowBall(int throwerId, Vector3 target, float power)
    {
        Debug.Log("We should extrapolate the position of the ball considering the time needed for the packet to arrive");
        //Check if ClientThrowBall was already called to avoid setting ball speed twice
        if (throwerId == BallState.GetAttachedPlayerID())
        {
            BallState.SetAttached(-1);
            if (BallState.ListenToServer)
            {
                SetBallSpeed(target, power);
            }
        }

    }


    private void SetBallSpeed(Vector3 target, float power)
    {
        ball.GetComponent<BallMovementView>().Throw(target, power);
        AttractionBall.DeactivatePlayer(gameObject);
        tryingToCatchBall = false;
        Invoke("ReactivateAttraction", 0.5f);
    }

    private void ReactivateAttraction()
    {
        tryingToCatchBall = true;
        AttractionBall.ActivatePlayer(gameObject);
    }
}


