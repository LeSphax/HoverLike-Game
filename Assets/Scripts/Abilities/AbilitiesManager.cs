using Ball;
using Byn.Net;
using PlayerManagement;
using System.Collections.Generic;
using TimeSlow;
using UnityEngine;

namespace AbilitiesManagement
{
    [RequireComponent(typeof(EffectsManager))]
    public class AbilitiesManager : SlideBall.NetworkMonoBehaviour
    {
        public event PlayerHasShotEventHandler PlayerHasShot;

        internal PlayerController controller;

        public AnimationCurve jumpCurve;

        private const float BLOCK_POWER = 250;

        private EffectsManager effectsManager;
        internal EffectsManager EffectsManager
        {
            get
            {
                if (effectsManager == null)
                {
                    effectsManager = GetComponent<EffectsManager>();
                }
                return effectsManager;
            }
        }

        public void ResetPersistentEffects()
        {
            foreach (var effect in persistentEffects)
            {
                effect.StopEffect();
            }
            persistentEffects.Clear();
        }

        public static void ResetVisualEffects()
        {
            foreach (var vfx in visualEffects)
            {
                vfx.ClearEffect();
            }
        }

        public void ResetAllEffects()
        {
            ResetVisualEffects();
            ResetPersistentEffects();
        }

        public static List<AVisualEffect> visualEffects = new List<AVisualEffect>();
        internal List<PersistentEffect> persistentEffects = new List<PersistentEffect>();

        protected void Awake()
        {
            controller = GetComponent<PlayerController>();
            myRigidbody = GetComponent<Rigidbody>();
        }

        private Rigidbody myRigidbody;
        private Vector3 summedDirections;
        private Vector3 lastDirection;
        private bool moveUpdated = false;
        private int framesWithoutMoveUpdate;

        [MyRPC]
        internal void Move(Vector3 direction, Vector3 position)
        {
            if (CanUseAbility())
            {
                lastDirection = direction;
                Vector3 previousRotation = controller.transform.rotation.eulerAngles;
                transform.LookAt(position);
                transform.rotation = Quaternion.Euler(previousRotation.x, controller.transform.rotation.eulerAngles.y, previousRotation.z);
                summedDirections += direction;
                moveUpdated = true;
            }
        }



        private void FixedUpdate()
        {
            if (NetworkingState.IsServer)
            {
                if (!moveUpdated)
                {
                    summedDirections = lastDirection;
                }
                if (controller.Player.State.Movement == MovementState.PLAYING)
                {
                    if (controller.Player.AvatarSettingsType == AvatarSettings.AvatarSettingsTypes.ATTACKER)
                    {
                        myRigidbody.AddForce(summedDirections.normalized * 100, ForceMode.Acceleration);
                        myRigidbody.velocity *= Mathf.Min(1.0f, 75 / myRigidbody.velocity.magnitude);
                    }
                    else
                    {
                        myRigidbody.velocity = summedDirections.normalized * 30 * (1 + 0.3f * inZone);
                        //transform.position += direction.normalized * 0.4f * (1 + 0.3f * inZone);
                    }
                }
                moveUpdated = false;
                summedDirections = Vector3.zero;
            }
        }

        int inZone;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == Tags.GoalZone)
            {
                inZone = 1;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == Tags.GoalZone)
            {
                inZone = 0;
            }
        }

        [MyRPC]
        internal void Arm(bool isArming)
        {
            if (CanUseAbility() && controller.Player.HasBall)
            {
                EffectsManager.View.RPC("ShowArmAnimation", RPCTargets.All, isArming);
            }
        }

        [MyRPC]
        internal void ShootCurved(Vector3[] controlPoints, float power)
        {
            if (CanUseAbility() && controller.Player.HasBall)
            {
                controller.ballController.ThrowBallCurve(controlPoints, power);
                EffectsManager.View.RPC("ThrowBall", RPCTargets.All);
            }
        }

        [MyRPC]
        internal void Shoot(Vector3 target, float power)
        {
            if (CanUseAbility() && controller.Player.HasBall)
            {
                controller.ballController.ThrowBall(target, power);
                EffectsManager.View.RPC("ThrowBall", RPCTargets.All);
                if (PlayerHasShot != null)
                    PlayerHasShot.Invoke(power);
            }
        }

        [MyRPC]
        internal void Dash(Vector3 position)
        {
            if (CanUseAbility())
            {
                new DashPersistentEffect(this, position, controller.movementManager.MaxPlayerVelocity);
                new StealPersistentEffect(this, DashPersistentEffect.dashDuration * 2);
                EffectsManager.View.RPC("ShowSmoke", RPCTargets.All);
            }

        }

        [MyRPC]
        internal void Jump()
        {
            if (CanUseAbility())
            {
                new JumpPersistentEffect(this, jumpCurve);
            }
            //controller.movementManager.Jump();
        }

        [MyRPC]
        private void Brake(bool activate)
        {
            //controller.movementManager.Brake(activate);
        }


        private bool CanUseAbility()
        {
            return controller.Player.State.Movement == MovementState.PLAYING;
        }

        [MyRPC]
        private void Steal()
        {
            if (CanUseAbility())
            {
                if (controller.Player.HasBall)
                {
                    new ProtectionPersistentEffect(this);
                    EffectsManager.View.RPC("ShowProtection", RPCTargets.All);
                }
                else
                {
                    new StealPersistentEffect(this);
                    EffectsManager.View.RPC("ShowStealing", RPCTargets.All);
                }
            }
        }

        [MyRPC]
        private void Pass(ConnectionId targetId)
        {
            if (CanUseAbility() && controller.Player.HasBall && controller.Player.AvatarSettingsType == AvatarSettings.AvatarSettingsTypes.ATTACKER)
            {
                MyComponents.BallState.trajectoryStrategy = new PassTrajectoryStrategy(MyComponents.BallState, MyComponents.Players.players, controller.playerConnectionId, targetId);
                EffectsManager.View.RPC("ThrowBall", RPCTargets.All);
            }
        }

        [MyRPC]
        private void Teleport()
        {
            if (CanUseAbility() && controller.Player.AvatarSettingsType == AvatarSettings.AvatarSettingsTypes.GOALIE)
            {
                MyComponents.NetworkViewsManagement.Instantiate("Effects/Teleportation", transform.position, Quaternion.identity);
                MyComponents.NetworkViewsManagement.Instantiate("Effects/Teleportation", controller.Player.SpawningPoint, Quaternion.identity);
                new TeleportPersistentEffect(this);
            }
        }

        [MyRPC]
        private void Block()
        {
            if (CanUseAbility() && controller.Player.AvatarSettingsType == AvatarSettings.AvatarSettingsTypes.GOALIE)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, BlockExplosion.BLOCK_DIAMETER / 2, LayersGetter.BallMask());
                foreach (Collider hit in colliders)
                {
                    if (hit.tag == Tags.Ball)
                    {
                        Rigidbody rb = hit.GetComponent<Rigidbody>();
                        Vector3 force = hit.transform.position - transform.position;
                        force.Normalize();
                        if (rb != null)
                        {
                            rb.AddForce(force * BLOCK_POWER, ForceMode.VelocityChange);
                        }
                        if (MyComponents.BallState.trajectoryStrategy.GetType() == typeof(ThrowTrajectoryStrategy))
                        {
                            MyComponents.BallState.trajectoryStrategy = new FreeTrajectoryStrategy(MyComponents.BallState);
                        }
                    }
                }
                MyComponents.NetworkViewsManagement.Instantiate("Effects/BlockExplosion", Vector3.zero, Quaternion.identity, controller.playerConnectionId);
            }
        }

        [MyRPC]
        private void TimeSlow(Vector3 epicenter)
        {
            if (CanUseAbility() && controller.Player.AvatarSettingsType == AvatarSettings.AvatarSettingsTypes.GOALIE)
            {
                MyComponents.NetworkViewsManagement.Instantiate("Effects/TimeSlow", epicenter, Quaternion.identity, Colors.Teams[(int)controller.Player.Team]);
                new TimeSlowPersistentEffect(this, epicenter);
            }
        }

        public void ApplyAbilityEffects(float dt)
        {
            foreach (var effect in new List<PersistentEffect>(persistentEffects))
            {
                effect.ApplyEffect(dt);
            }
        }



    }
}