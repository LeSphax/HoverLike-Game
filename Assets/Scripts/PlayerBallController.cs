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
            playerConnectionId = id;
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
                        MyComponents.BallState.SetAttached(playerConnectionId);
                        stealing = false;
                        break;
                    }
                }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (MyComponents.NetworkManagement.isServer)
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
            if (MyComponents.NetworkManagement.isServer)
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
            if (MyComponents.NetworkManagement.isServer)
            {
                if (collider.gameObject.tag == Tags.CatchDetector && !MyComponents.BallState.IsAttached() && tryingToCatchBall && (!MyComponents.BallState.UnCatchable || stealing || MyComponents.BallState.PassTarget == playerConnectionId))
                {
                    Assert.IsTrue(playerConnectionId != BallState.NO_PLAYER_ID);
                    MyComponents.BallState.SetAttached(playerConnectionId);
                }
            }
        }

        public void ThrowBall(Vector3 target, float power)
        {
            if (playerConnectionId == MyComponents.BallState.GetIdOfPlayerOwningBall())
            {
                MyComponents.BallState.DetachBall();
                SetBallSpeed(target, power);
            }
        }

        public void SetBallSpeed(Vector3 target, float power)
        {
            PrepareForThrowing();
            Ball.GetComponent<BallMovementView>().Throw(target, power);
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

