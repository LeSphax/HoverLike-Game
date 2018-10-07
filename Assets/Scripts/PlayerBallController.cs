using Ball;
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

        private List<ConnectionId> idsPlayerInContact;

        private ShootInput shootInput;

        private GameObject Ball
        {
            get
            {
                return MyComponents.BallState.gameObject;
            }
        }


        protected void Start()
        {
            MyComponents.GameInit.AllObjectsCreated += StartGame;
            Player.eventNotifier.ListenToEvents(TryStealing, PlayerFlags.STEALING_STATE);
        }

        public void Init(ConnectionId id)
        {
            playerConnectionId = id;
        }

        private void StartGame()
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), Ball.GetComponent<Collider>(), true);
        }

        public void Reset()
        {
            idsPlayerInContact = new List<ConnectionId>();
        }

        private void TryStealing(Player player = null)
        {
            if (Player.State.Stealing == StealingState.STEALING)
                foreach (ConnectionId id in idsPlayerInContact)
                {
                    if (id == MyComponents.BallState.GetIdOfPlayerOwningBall() && MyComponents.Players.players[id].State.Stealing != StealingState.PROTECTED)
                    {
                        MyComponents.BallState.SetAttached(playerConnectionId);
                        Player.State.Stealing = StealingState.IDLE;
                        break;
                    }
                }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (NetworkingState.IsServer)
            {
                if (Tags.IsPlayer(collision.gameObject.tag))
                {
                    //Debug.Log(this + " OnCollisionEnter " + collision.gameObject.name);
                    idsPlayerInContact.Add(collision.gameObject.GetComponent<PlayerController>().playerConnectionId);
                    TryStealing();
                }
            }
        }

        void OnCollisionExit(Collision collision)
        {
            if (NetworkingState.IsServer)
            {
                if (Tags.IsPlayer(collision.gameObject.tag))
                {
                    //Debug.Log(this + " OnCollisionExit " + collision.gameObject.name);
                    idsPlayerInContact.Remove(collision.gameObject.GetComponent<PlayerController>().playerConnectionId);
                }
            }
        }

        void OnTriggerStay(Collider collider)
        {
            if (NetworkingState.IsServer)
            {
                if (!MyComponents.BallState.IsAttached())
                {
                    if (collider.gameObject.tag == Tags.CatchDetector)
                    {
                        if (tryingToCatchBall && !MyComponents.BallState.UnCatchable)
                        {
                            Assert.IsTrue(playerConnectionId != BallState.NO_PLAYER_ID);
                            MyComponents.BallState.SetAttached(playerConnectionId);
                        }
                    }
                    else if (collider.gameObject.tag == Tags.ProtectionSphere)
                    {
                        Debug.Log("ProtectionSphere" + MyComponents.BallState.UnCatchable + "  " + MyComponents.BallState.PassTarget + "  " + playerConnectionId);
                        if (MyComponents.BallState.UnCatchable && (Player.State.Stealing == StealingState.STEALING || MyComponents.BallState.PassTarget == playerConnectionId))
                        {
                            Debug.Log("ProtectionSphere OK");
                            Assert.IsTrue(playerConnectionId != BallState.NO_PLAYER_ID);
                            MyComponents.BallState.SetAttached(playerConnectionId);
                        }
                    }
                }
            }
        }

        public void ThrowBallCurve(Vector3[] controlPoints, float power)
        {
            PrepareForThrowing();
            MyComponents.BallState.trajectoryStrategy = new ThrowTrajectoryStrategy(MyComponents.BallState, controlPoints, power);
        }

        public void ThrowBall(Vector3 target, float power)
        {
            MyComponents.BallState.trajectoryStrategy = new FreeTrajectoryStrategy(MyComponents.BallState);
            PrepareForThrowing();
            SetBallSpeed(target, power);
        }

        public void SetBallSpeed(Vector3 target, float power)
        {
            Ball.GetComponent<BallMovementView>().Throw(target, power);
        }

        private void PrepareForThrowing()
        {
            tryingToCatchBall = false;
            Invoke("ReactivateAttraction", 0.5f);
        }

        private void ReactivateAttraction()
        {
            tryingToCatchBall = true;
        }

        private void OnDestroy()
        {
            if (Player != null)
                Player.eventNotifier.StopListeningToEvents(TryStealing, PlayerFlags.STEALING_STATE);
        }
    }
}

