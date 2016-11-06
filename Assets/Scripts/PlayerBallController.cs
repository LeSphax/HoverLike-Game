using Byn.Net;
using PlayerManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace PlayerBallControl
{
    public class PlayerBallController : PlayerView
    {

        private PowerBar powerBar;
        private bool tryingToCatchBall = true;

        private List<ConnectionId> idsPlayerInContact = new List<ConnectionId>();

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
                return MyComponents.BallState.gameObject;
            }
        }


        protected void Start()
        {
            MyComponents.GameInitialization.AllObjectsCreated += StartGame;
        }

        public void Init(ConnectionId id)
        {
            this.playerConnectionId = id;
        }

        private void StartGame()
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), Ball.GetComponent<Collider>(), true);
        }

        private void TryStealing()
        {
            if (stealing)
                foreach (ConnectionId id in idsPlayerInContact)
                {
                    if (id == MyComponents.BallState.GetIdOfPlayerOwningBall())
                    {
                        View.RPC("StealBall", RPCTargets.Server, MyComponents.BallState.GetIdOfPlayerOwningBall());
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
                    idsPlayerInContact.Add(collision.gameObject.GetComponent<PlayerController>().playerConnectionId);
                    TryStealing();
                    Debug.Log(gameObject.name + " CollisionEnter " + collision.gameObject.name);
                }
            }
        }

        void OnCollisionExit(Collision collision)
        {
            if (View.isMine)
            {
                if (Tags.IsPlayer(collision.gameObject.tag))
                {
                    idsPlayerInContact.Remove(collision.gameObject.GetComponent<PlayerController>().playerConnectionId);
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
                if (collider.gameObject.tag == Tags.CatchDetector && !MyComponents.BallState.IsAttached() && tryingToCatchBall)
                {
                    //Debug.LogError("Enter" + collider);
                    View.RPC("PickUpBall", RPCTargets.Server);
                }
            }
        }

        [MyRPC]
        public void PickUpBall()
        {
            Assert.IsTrue(playerConnectionId != BallState.NO_PLAYER_ID);
            Debug.Log("PickUpBall " + gameObject.name + "   " + !MyComponents.BallState.IsAttached() + " " + !MyComponents.BallState.Uncatchable);
            Assert.IsTrue(MyComponents.NetworkManagement.isServer);
            if (!MyComponents.BallState.IsAttached() && !MyComponents.BallState.Uncatchable)
            {
                MyComponents.BallState.SetAttached(playerConnectionId);
            }
        }

        [MyRPC]
        private void StealBall(ConnectionId victimId)
        {
            Assert.IsTrue(playerConnectionId != BallState.NO_PLAYER_ID);
            Debug.Log("StealBall " + victimId + "     " + playerConnectionId + "   " + gameObject.name);
            Assert.IsTrue(MyComponents.NetworkManagement.isServer);
            if (MyComponents.BallState.GetIdOfPlayerOwningBall() == victimId)
            {
                Debug.LogWarning("SetAttached " + playerConnectionId);
                MyComponents.BallState.SetAttached(playerConnectionId);
            }
        }

        internal void ClientThrowBall(Vector3 target, float power)
        {
            MyComponents.BallState.AttachBall(BallState.NO_PLAYER_ID);
            SetBallSpeed(target, power);
            MyComponents.BallState.ListenToServer = false;
            View.RPC("ServerThrowBall", RPCTargets.Server, playerConnectionId, target, power);
        }

        [MyRPC]
        private void ServerThrowBall(ConnectionId throwerId, Vector3 target, float power)
        {
            Debug.Log("We should extrapolate the position of the ball considering the time needed for the packet to arrive");
            //Check if ClientThrowBall was already called to avoid setting ball speed twice
            if (throwerId == MyComponents.BallState.GetIdOfPlayerOwningBall())
            {
                MyComponents.BallState.SetAttached(BallState.NO_PLAYER_ID);
                if (MyComponents.BallState.ListenToServer)
                {
                    SetBallSpeed(target, power, MyComponents.TimeManagement.GetLatencyInMiliseconds(throwerId) / 1000);
                }
            }

        }


        private void SetBallSpeed(Vector3 target, float power, float latencyInSeconds = 0)
        {
            PrepareForThrowing();
            Ball.GetComponent<BallMovementView>().Throw(target, power, latencyInSeconds);
        }

        private void PrepareForThrowing()
        {
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
}

