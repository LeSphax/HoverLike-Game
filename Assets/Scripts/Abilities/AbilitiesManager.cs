using PlayerManagement;
using System;
using System.Collections.Generic;
using TimeSlow;
using UnityEngine;

namespace AbilitiesManagement
{
    public class AbilitiesManager : SlideBall.MonoBehaviour
    {
        internal PlayerController controller;

        private const float BLOCK_POWER = 100;

        public void ResetPersistentEffects()
        {
            persistentEffects.Clear();
        }

        public static void ResetVisualEffects()
        {
            foreach (var go in visualEffects)
            {
                Destroy(go);
            }
        }

        public static void ResetAllEffects()
        {
            ResetVisualEffects();
            foreach(var player in Players.players.Values)
            {
                player.controller.abilitiesManager.ResetPersistentEffects();
            }
        }

        public static List<GameObject> visualEffects = new List<GameObject>();
        internal List<PersistentEffect> persistentEffects = new List<PersistentEffect>();

        protected void Awake()
        {
            controller = GetComponent<PlayerController>();
        }

        [MyRPC]
        private void Dash(Vector3 position)
        {
            new DashPersistentEffect(this, position);
        }

        [MyRPC]
        private void Steal(float duration)
        {
            new StealPersistentEffect(this, duration);
        }

        [MyRPC]
        private void Pass(Vector3 position)
        {
            new PassPersistentEffect(this, position);
        }

        [MyRPC]
        private void Teleport()
        {
            MyComponents.NetworkViewsManagement.Instantiate("Effects/Teleportation", transform.position, Quaternion.identity);
            MyComponents.NetworkViewsManagement.Instantiate("Effects/Teleportation", controller.Player.SpawningPoint, Quaternion.identity);
            new TeleportPersistentEffect(this);
        }

        [MyRPC]
        private void Block()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, BlockExplosion.BLOCK_DIAMETER/2, LayersGetter.BallMask());
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
            MyComponents.NetworkViewsManagement.Instantiate("Effects/BlockExplosion", controller.playerConnectionId);
        }

        [MyRPC]
        private void TimeSlow(Vector3 epicenter)
        {
            MyComponents.NetworkViewsManagement.Instantiate("Effects/TimeSlow", epicenter, Quaternion.identity);
            new TimeSlowPersistentEffect(this, epicenter);
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