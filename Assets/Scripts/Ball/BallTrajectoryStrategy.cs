using UnityEngine;

namespace Ball
{
    public abstract class BallTrajectoryStrategy
    {
        protected BallState BallState;

        public abstract void MoveBall();
        public virtual Vector3 CurrentVelocity {
            get
            {
                return Vector3.zero;
            }
        }

        public BallTrajectoryStrategy(BallState ballState)
        {
            BallState = ballState;
        }
    }


}