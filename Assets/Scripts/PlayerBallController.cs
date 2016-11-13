using Byn.Net;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace PlayerBallControl
{
    public class PlayerBallController : PlayerView
    {

        private const float ATTRACTION_RANGE = 20f;
        private const float ATTRACTION_POWER = 50f;
        private const float CATCH_RANGE = 5f;

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

        public void Init(ConnectionId id)
        {
            this.playerConnectionId = id;
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

        //void OnTriggerEnter(Collider collider)
        //{
        //    if (View.isMine)
        //    {
        //        if (collider.tag == Tags.CatchDetector && tryingToCatchBall)
        //        {
        //            if (!MyComponents.BallState.IsAttached())
        //            {
        //                Debug.LogError("Enter" + collider);
        //                View.RPC("PickUpBall", RPCTargets.Server);
        //            }
        //        }
        //    }
        //}

        [MyRPC]
        public void PickUpBall()
        {
            Assert.IsTrue(playerConnectionId != BallState.NO_PLAYER_ID);
            Debug.LogError("PickUpBall " + gameObject.name + "   " + !MyComponents.BallState.IsAttached() + " " + !MyComponents.BallState.Uncatchable);
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
            if (!MyComponents.NetworkManagement.isServer)
            {
                MyComponents.BallState.AttachBall(BallState.NO_PLAYER_ID);
                ThrowBall(target, power);
            }
            View.RPC("ServerThrowBall", RPCTargets.Server, playerConnectionId, target, power);
        }

        [MyRPC]
        private void ServerThrowBall(ConnectionId throwerId, Vector3 target, float power)
        {
            if (throwerId == MyComponents.BallState.GetIdOfPlayerOwningBall())
            {
                ThrowBall(target, power, MyComponents.TimeManagement.GetLatencyInMiliseconds(throwerId) / 1000);
                MyComponents.BallState.SetAttached(BallState.NO_PLAYER_ID);
            }
        }

        internal void TryCatchBall(Vector3 modelPosition)
        {
            if ((!MyComponents.BallState.IsAttached() || Stealing) && !MyComponents.BallState.Uncatchable && tryingToCatchBall)
            {
                float distanceFromBall = Vector3.Distance(MyComponents.BallState.ballModel.transform.position, modelPosition);
                if (distanceFromBall <= CATCH_RANGE)
                {

                    MyComponents.BallState.SetAttached(playerConnectionId);
                }
            }
        }

        public void TryAttractBall(Vector3 modelPosition)
        {
            if (!MyComponents.BallState.Uncatchable && tryingToCatchBall && !MyComponents.BallState.IsAttached())
            {
                float distanceFromBall = Vector3.Distance(MyComponents.BallState.ballModel.transform.position, modelPosition);
                if (distanceFromBall <= ATTRACTION_RANGE)
                {
                    GameObject ballModel = MyComponents.BallState.ballModel;
                    Vector3 ballPosition = ballModel.transform.position;
                    //
                    Vector3 velocity = new Vector3(modelPosition.x - ballPosition.x, 0, modelPosition.z - ballPosition.z);
                    velocity.Normalize();
                    //
                    ballModel.GetComponent<CustomRigidbody>().AddForce(velocity * ATTRACTION_POWER);
                }
            }
        }


        private void ThrowBall(Vector3 target, float power, float latencyInSeconds = 0)
        {
            tryingToCatchBall = false;
            MyComponents.BallState.ballPhysics.Throw(target, power, latencyInSeconds);
            Invoke("TryToCatchBall", 0.5f);
        }

        void TryToCatchBall()
        {
            tryingToCatchBall = true;
        }
    }
}

