using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(PowerBar))]
public class PlayerBallController : SlideBall.MonoBehaviour
{

    private PowerBar powerBar;
    private bool tryingToCatchBall = true;

    private List<int> idsPlayerInContact = new List<int>();

    private ShootInput shootInput;

    private bool stealing;
    public bool Stealing
    {
        get
        {
            return stealing;
        }
        set
        {
            stealing = value;
            TryStealing();
        }
    }

    private GameObject Ball
    {
        get
        {
            return MyGameObjects.BallState.Ball;
        }
    }


    protected void Start()
    {
        shootInput = gameObject.AddComponent<ShootInput>();
        MyGameObjects.GameInitialization.AllObjectsCreated += StartGame;
        powerBar = GetComponent<PowerBar>();
        MyGameObjects.Properties.AddListener(PropertiesKeys.NamePlayerHoldingBall, AttachBall);
    }

    private void StartGame()
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), Ball.GetComponent<Collider>(), true);
    } 

    private void TryStealing()
    {
        if (stealing)
            foreach (int id in idsPlayerInContact)
            {
                if (id == MyGameObjects.BallState.GetIdOfPlayerOwningBall())
                {
                    View.RPC("StealBall", RPCTargets.Server, MyGameObjects.BallState.GetIdOfPlayerOwningBall());
                    stealing = false;
                    break;
                }
            }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (View.isMine)
        {
            if (Tags.IsPlayer(collision.gameObject.tag))
            {
                idsPlayerInContact.Add(collision.gameObject.GetNetworkView().ViewId);
                TryStealing();
                Debug.Log("CollisionEnter");
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (View.isMine)
        {
            if (Tags.IsPlayer(collision.gameObject.tag))
            {
                idsPlayerInContact.Remove(collision.gameObject.GetNetworkView().ViewId);
                Debug.Log("CollisionExit");
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (View.isMine)
        {
            if (collider.gameObject.tag == Tags.CatchDetector)
            {
                //Debug.LogError("Exit" + collider);
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (View.isMine)
        {
            if (collider.gameObject.tag == Tags.CatchDetector && !MyGameObjects.BallState.IsAttached() && tryingToCatchBall)
            {
                //Debug.LogError("Enter" + collider);
                View.RPC("PickUpBall", RPCTargets.Server);
            }
        }
    }

    [MyRPC]
    public void PickUpBall()
    {
        Debug.Log("PickUpBall " + gameObject.name + "   " + MyGameObjects.BallState.IsAttached());
        Assert.IsTrue(MyGameObjects.NetworkManagement.isServer);
        if (!MyGameObjects.BallState.IsAttached())
        {
            MyGameObjects.BallState.SetAttached(View.ViewId);
        }
    }

    [MyRPC]
    private void StealBall(int victimId)
    {
        Debug.Log("StealBall " + victimId + "     " + View.ViewId + "   " + gameObject.name);
        Assert.IsTrue(MyGameObjects.NetworkManagement.isServer);
        if (MyGameObjects.BallState.GetIdOfPlayerOwningBall() == victimId)
        {
            Debug.LogWarning("SetAttached " + View.ViewId);
            MyGameObjects.BallState.SetAttached(View.ViewId);
        }
    }

    public void AttachBall(object previousPlayer, object newPlayer)
    {
        int newPlayerId = newPlayer == null ? -1 : (int)newPlayer;
        int previousPlayerId = previousPlayer == null ? -1 : (int)previousPlayer;

        bool attach = newPlayerId == View.ViewId && previousPlayerId != View.ViewId;
        bool detach = previousPlayerId == View.ViewId && newPlayerId == -1;

        Assert.IsFalse(attach && detach);

        if (attach)
        {
            MyGameObjects.BallState.AttachBall(View.ViewId);
        }
        else if (detach)
        {
            MyGameObjects.BallState.AttachBall(-1);
        }
    }

    void Update()
    {
        if (View.isMine && MyGameObjects.BallState != null)
        {
            UpdateThrow();
        }
    }

    private void UpdateThrow()
    {
        if (shootInput.Activate())
        {
            if (MyGameObjects.BallState.GetIdOfPlayerOwningBall() == View.ViewId)
            {
                powerBar.StartFilling();
            }
        }
        else if (shootInput.Reactivate() && powerBar.IsFilling())
        {
            if (MyGameObjects.BallState.GetIdOfPlayerOwningBall() == View.ViewId)
            {
                ClientThrowBall(Functions.GetMouseWorldPosition(), powerBar.powerValue);
                powerBar.Hide();
            }
        }
        else if (shootInput.Cancel() && powerBar.IsFilling())
        {
            powerBar.Hide();
        }
    }

    private void ClientThrowBall(Vector3 target, float power)
    {
        MyGameObjects.BallState.AttachBall(-1);
        SetBallSpeed(target, power);
        MyGameObjects.BallState.ListenToServer = false;
        View.RPC("ServerThrowBall", RPCTargets.Server, View.ViewId, target, power);
    }

    [MyRPC]
    private void ServerThrowBall(short throwerId, Vector3 target, float power)
    {
        Debug.Log("We should extrapolate the position of the ball considering the time needed for the packet to arrive");
        //Check if ClientThrowBall was already called to avoid setting ball speed twice
        if (throwerId == MyGameObjects.BallState.GetIdOfPlayerOwningBall())
        {
            MyGameObjects.BallState.SetAttached(-1);
            if (MyGameObjects.BallState.ListenToServer)
            {
                SetBallSpeed(target, power);
            }
        }

    }


    private void SetBallSpeed(Vector3 target, float power)
    {
        Ball.GetComponent<BallMovementView>().Throw(target, power);
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


