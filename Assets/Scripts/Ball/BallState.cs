using Byn.Net;
using PlayerManagement;
using System;
using UnityEngine;

namespace Ball
{
    public delegate void UncatchableChangeHandler(bool uncatchable);

    public class BallState : SlideBall.NetworkMonoBehaviour
    {
        private Rigidbody mRigidbody;
        public Rigidbody Rigidbody
        {
            get
            {
                if (mRigidbody == null)
                {
                    mRigidbody = GetComponent<Rigidbody>();
                }
                return mRigidbody;
            }
        }

        public event UncatchableChangeHandler UncatchableChanged;

        public static ConnectionId NO_PLAYER_ID
        {
            get
            {
                return Players.INVALID_PLAYER_ID;
            }
        }

        [NonSerialized]
        public ConnectionId IdPlayerOwningBall = NO_PLAYER_ID;
        [NonSerialized]
        public ConnectionId PassTarget = NO_PLAYER_ID;

        private bool uncatchable;
        public bool UnCatchable
        {
            get
            {
                return uncatchable;
            }
            set
            {
                View.RPC("UpdateUncatchable", RPCTargets.All, value);
            }
        }

        private void MakeBallUncatchable(bool uncatchable)
        {
            if (uncatchable)
            {
                protectionSphere.SetActive(true);
            }
            else
            {
                protectionSphere.SetActive(false);
            }
            TrySetKinematic();
        }

        internal void TrySetKinematic()
        {
            if (NetworkingState.IsServer)
            {
                if (uncatchable || IsAttached())
                    Rigidbody.isKinematic = true;
                else
                    Rigidbody.isKinematic = false;
            }
        }

        [MyRPC]
        private void UpdateUncatchable(bool newValue)
        {
            this.uncatchable = newValue;
            MakeBallUncatchable(newValue);
            if (UncatchableChanged != null)
                UncatchableChanged.Invoke(newValue);
        }


        public GameObject protectionSphere;
        public AttractionBall attraction;

        public BallTrajectoryStrategy trajectoryStrategy;

        void Start()
        {
            MyComponents.GameInit.AddGameStartedListener(StartGame);
            trajectoryStrategy = new FreeTrajectoryStrategy(MyComponents.BallState);
            MakeBallUncatchable(UnCatchable);
            TrySetKinematic();
        }

        private void FixedUpdate()
        {
            if (NetworkingState.IsServer)
                trajectoryStrategy.MoveBall();
        }

        public void StartGame()
        {
            if (NetworkingState.IsServer)
            {
                IdPlayerOwningBall = NO_PLAYER_ID;
            }
            AttachBall(GetIdOfPlayerOwningBall());
        }

        public void SetAttached(ConnectionId playerId, bool sendUpdate = true)
        {
            if (playerId != IdPlayerOwningBall)
            {
                ConnectionId previousId = IdPlayerOwningBall;
                IdPlayerOwningBall = playerId;
                MyComponents.Players.ChangePlayerOwningBall(previousId, playerId, sendUpdate);
            }
        }
        public void DetachBall()
        {
            SetAttached(NO_PLAYER_ID);
        }

        public bool IsAttached()
        {
            return GetIdOfPlayerOwningBall() != NO_PLAYER_ID;
        }

        public ConnectionId GetIdOfPlayerOwningBall()
        {
            return IdPlayerOwningBall;
        }

        public PlayerController GetAttachedPlayer()
        {
            foreach (Player player in MyComponents.Players.players.Values)
            {
                if (player.id == GetIdOfPlayerOwningBall())
                    return player.controller;
            }
            return null;
        }


        public void AttachBall(ConnectionId playerId)
        {
            bool attach = playerId != NO_PLAYER_ID;
            if (attach)
            {
                trajectoryStrategy = new AttachedTrajectoryStrategy(MyComponents.BallState);
            }
            else if (!NetworkingState.IsServer)
            {
                trajectoryStrategy = new FreeTrajectoryStrategy(MyComponents.BallState);
            }
        }

        [MyRPC]
        internal void PutAtStartPosition()
        {
            if (NetworkingState.IsServer)
            {
                trajectoryStrategy = new FreeTrajectoryStrategy(MyComponents.BallState);
                View.RPC("PutAtStartPosition", RPCTargets.Others);
            }
            PutBallAtPosition(MyComponents.Spawns.BallSpawn);
        }

        public void PutBallAtPosition(Vector3 position)
        {
            gameObject.transform.position = position;
            if (NetworkingState.IsServer)
            {
                Rigidbody.velocity = Vector3.zero;
                gameObject.GetComponentInChildren<AttractionBall>().Reset();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.tag != Tags.Ground)
                if (trajectoryStrategy.GetType() == typeof(ThrowTrajectoryStrategy))
                {
                    //Debug.Log(collision.collider.tag);
                    //Debug.Log(collision.collider.name);
                    //EditorApplication.isPaused = true;
                    Rigidbody.velocity = trajectoryStrategy.CurrentVelocity;
                    trajectoryStrategy = new FreeTrajectoryStrategy(MyComponents.BallState);
                }
        }

        public void SetWorldAsParent()
        {
            transform.SetParent(MyComponents.transform);
        }
    }

}