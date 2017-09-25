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
            MyComponents.GameInitialization.AllObjectsCreated += StartGame;
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
                    if (id == MyComponents.BallState.GetIdOfPlayerOwningBall() && Players.players[id].State.Stealing != StealingState.PROTECTED)
                    {
                        MyComponents.BallState.SetAttached(playerConnectionId);
                        Player.State.Stealing = StealingState.IDLE;
                        break;
                    }
                }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (MyComponents.NetworkManagement.IsServer)
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
            if (MyComponents.NetworkManagement.IsServer)
            {
                if (Tags.IsPlayer(collision.gameObject.tag))
                {
                    //Debug.Log(this + " OnCollisionExit " + collision.gameObject.name);
                    idsPlayerInContact.Remove(collision.gameObject.GetComponent<PlayerController>().playerConnectionId);
                }
            }
        }

        void OnTriggerEnter(Collider collider)
        {
            if (MyComponents.NetworkManagement.IsServer)
            {
                if (collider.gameObject.tag == Tags.CatchDetector && !MyComponents.BallState.IsAttached())
                    if (tryingToCatchBall && (!MyComponents.BallState.UnCatchable || Player.State.Stealing == StealingState.STEALING || MyComponents.BallState.PassTarget == playerConnectionId))
                    {
                        Assert.IsTrue(playerConnectionId != BallState.NO_PLAYER_ID);
                        MyComponents.BallState.SetAttached(playerConnectionId);
                    }
            }
        }

        public void ThrowBallCurve(Vector3[] controlPoints, float power)
        {
            MyComponents.BallState.trajectoryStrategy = new ThrowTrajectoryStrategy(controlPoints, power);
        }

        public void ThrowBall(Vector3 target, float power)
        {
            MyComponents.BallState.trajectoryStrategy = new FreeTrajectoryStrategy();
            SetBallSpeed(target, power);
        }

        public void SetBallSpeed(Vector3 target, float power)
        {
            PrepareForThrowing();
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
    }
}

