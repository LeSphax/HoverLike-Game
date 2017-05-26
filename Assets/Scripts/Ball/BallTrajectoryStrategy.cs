using UnityEngine;

namespace Ball
{
    public abstract class BallTrajectoryStrategy
    {
        public abstract void MoveBall();
        public virtual Vector3 CurrentVelocity {
            get
            {
                return Vector3.zero;
            }
        }
    }


}