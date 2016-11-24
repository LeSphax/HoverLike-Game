﻿using System;
using AbilitiesManagement;
using PlayerManagement;
using UnityEngine;

public class TimeSlowEffect : AbilityEffect
{

    public override void ApplyOnTarget(GameObject targetGameObject, Vector3 epicenter)
    {
        Players.MyPlayer.controller.View.RPC("TimeSlow", RPCTargets.Server, epicenter);
    }



}

namespace TimeSlow
{
    public class TimeSlowPersistentEffect : PersistentEffect
    {
        Vector3 epicenter;
        Vector3 controlPosition;
        Vector3 startPosition;

        public const float DURATION = 5f;

        public TimeSlowPersistentEffect(AbilitiesManager manager, Vector3 epicenter) : base(manager)
        {
            this.epicenter = epicenter;

            duration = DURATION;
        }

        protected override void Apply(float dt)
        {
            Collider[] colliders = Physics.OverlapSphere(epicenter, TimeSlowTargeting.DIAMETER_TIME_SLOW_ZONE / 2, LayersGetter.AttackersMask() | LayersGetter.BallMask());
            foreach (var collider in colliders)
            {
                Rigidbody rb = GetObjectsWithRigidbody(collider.transform);
                if (rb != null)
                {
                    if (!TimeSlowApplier.ObjectsBeforeUpdate.ContainsKey(rb))
                        TimeSlowApplier.ObjectsBeforeUpdate.Add(rb, new RigidbodyState(rb.transform.position, rb.velocity));
                }
            }
        }

        protected override void StopEffect()
        {
        }

        private Rigidbody GetObjectsWithRigidbody(Transform t)
        {
            Rigidbody rigidbody = t.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                if (rigidbody.isKinematic)
                    return null;
                else
                    return rigidbody;
            }
            else if (t.parent != null)
                return GetObjectsWithRigidbody(t.parent);
            else
                return null;
        }
    }
}