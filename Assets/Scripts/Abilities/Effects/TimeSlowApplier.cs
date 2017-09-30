using System;
using System.Collections.Generic;
using UnityEngine;

namespace TimeSlow
{
    public class TimeSlowApplier : MonoBehaviour
    {
        public static float PlayerSlowProportion = 0.5f;
        public static float BallSlowProportion = 0.1f;
        internal static Dictionary<Rigidbody, RigidbodyState> ObjectsBeforeUpdate = new Dictionary<Rigidbody, RigidbodyState>();

        protected void Start()
        {
            LateFixedUpdate.evt += ApplySlow;
        }

        protected void ApplySlow()
        {
            foreach (var pair in ObjectsBeforeUpdate)
            {
                float slowProportion = Tags.IsPlayer(pair.Key.gameObject.tag) ? PlayerSlowProportion : BallSlowProportion;
                if (!pair.Key.isKinematic)
                {
                    Vector3 currentPosition = pair.Key.transform.position;
                    pair.Key.transform.position = currentPosition - ((currentPosition - pair.Value.position) * (1 - slowProportion));

                    Vector3 currentVelocity = pair.Key.velocity;
                    pair.Key.velocity = currentVelocity - ((currentVelocity - pair.Value.velocity) * (1 - slowProportion));
                }
            }
            ObjectsBeforeUpdate.Clear();
        }

    }

    internal class RigidbodyState
    {
        internal Vector3 position;
        internal Vector3 velocity;

        public RigidbodyState(Vector3 position, Vector3 velocity)
        {
            this.position = position;
            this.velocity = velocity;
        }
    }

}
