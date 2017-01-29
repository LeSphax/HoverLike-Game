using Byn.Net;
using PlayerManagement;
using System;
using System.Collections.Generic;
using TimeSlow;
using UnityEngine;

namespace AbilitiesManagement
{
    [RequireComponent(typeof(EffectsManager))]
    public class AbilitiesManager : SlideBall.MonoBehaviour
    {
        internal PlayerController controller;

        public AnimationCurve passCurve;
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
        }

        [MyRPC]
        private void Move(Vector2 position)
        {
            if (CanUseAbility())
                controller.targetManager.SetTarget(position);
        }

        [MyRPC]
        private void Shoot(Vector3 target, float power)
        {
            if (CanUseAbility())
            {
                controller.ballController.ThrowBall(target, power);
                EffectsManager.View.RPC("ThrowBall", RPCTargets.All);
            }
        }

        [MyRPC]
        private void Dash(Vector3 position)
        {
            if (CanUseAbility())
            {
                new DashPersistentEffect(this, position);
                new StealPersistentEffect(this, DashPersistentEffect.dashDuration * 2);
                EffectsManager.View.RPC("ShowSmoke", RPCTargets.All);
            }
        }

        [MyRPC]
        private void Jump()
        {
            if (CanUseAbility())
            {
                new JumpPersistentEffect(this, jumpCurve);
            }
            //controller.movementManager.Jump();
        }

        [MyRPC]
        private void Brake()
        {
            if (CanUseAbility())
                controller.movementManager.Brake();
        }


        private bool CanUseAbility()
        {
            return controller.Player.CurrentState == Player.State.PLAYING;
        }

        [MyRPC]
        private void Steal(float duration)
        {
            if (CanUseAbility())
            {
                new StealPersistentEffect(this, duration);
                EffectsManager.View.RPC("ShowStealing", RPCTargets.All);
            }
        }

        [MyRPC]
        private void Pass(ConnectionId id)
        {
            if (CanUseAbility())
            {
                new PassPersistentEffect(this, id, passCurve);
                EffectsManager.View.RPC("ThrowBall", RPCTargets.All);
            }
        }

        [MyRPC]
        private void Teleport()
        {
            if (CanUseAbility())
            {
                MyComponents.NetworkViewsManagement.Instantiate("Effects/Teleportation", transform.position, Quaternion.identity);
                MyComponents.NetworkViewsManagement.Instantiate("Effects/Teleportation", controller.Player.SpawningPoint, Quaternion.identity);
                new TeleportPersistentEffect(this);
            }
        }

        [MyRPC]
        private void Block()
        {
            if (CanUseAbility())
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

                    }
                }
                MyComponents.NetworkViewsManagement.Instantiate("Effects/BlockExplosion", Vector3.zero, Quaternion.identity, controller.playerConnectionId);
            }
        }

        [MyRPC]
        private void TimeSlow(Vector3 epicenter)
        {
            if (CanUseAbility())
            {
                MyComponents.NetworkViewsManagement.Instantiate("Effects/TimeSlow", epicenter, Quaternion.identity);
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