using UnityEngine;
using UnityEngine.Assertions;

namespace Ball
{
    public class AttachedTrajectoryStrategy : BallTrajectoryStrategy
    {

        public static Vector3 ballHoldingPosition = new Vector3(0f, 0f, 0f);
        public AttachedTrajectoryStrategy(BallState ballState) : base(ballState)
        {
            Assert.IsTrue(BallState.GetAttachedPlayer() != null);
            Transform hand = BallState.GetAttachedPlayer().PlayerMesh.hand;
            BallState.transform.SetParent(hand);

            if (NetworkingState.IsServer)
            {
                BallState.UnCatchable = false;
                BallState.TrySetKinematic();
                AttractionBall.Activated = false;
            }
            BallState.protectionSphere.SetActive(false);

            BallState.transform.localPosition = ballHoldingPosition;

        }

        public override void MoveBall()
        {
            //Do nothing
        }
    }
}
