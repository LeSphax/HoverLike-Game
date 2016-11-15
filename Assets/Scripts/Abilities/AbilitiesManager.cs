using System;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitiesManagement
{
    public class AbilitiesManager : SlideBall.MonoBehaviour
    {
        internal PlayerController controller;

        internal List<PersistentEffect> persistentEffects = new List<PersistentEffect>();

        protected void Awake()
        {
            controller = GetComponent<PlayerController>();
        }

        [MyRPC]
        private void Dash(Vector3 position)
        {
            new PersistentDashEffect(this, position);
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