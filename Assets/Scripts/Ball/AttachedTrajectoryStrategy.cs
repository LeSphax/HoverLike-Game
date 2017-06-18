using UnityEngine;
using UnityEngine.Assertions;

namespace Ball
{
    public class AttachedTrajectoryStrategy : BallTrajectoryStrategy
    {

        public static Vector3 ballHoldingPosition = new Vector3(0f, 0f, 0f);
        public AttachedTrajectoryStrategy()
        {
            Assert.IsTrue(MyComponents.BallState.GetAttachedPlayer() != null);
            Transform hand = MyComponents.BallState.GetAttachedPlayer().PlayerMesh.hand;
            MyComponents.BallState.transform.SetParent(hand);

            if (MyComponents.NetworkManagement.IsServer)
            {
                MyComponents.BallState.UnCatchable = false;
                MyComponents.BallState.TrySetKinematic();
                AttractionBall.Activated = false;
            }
            MyComponents.BallState.protectionSphere.SetActive(false);

            MyComponents.BallState.transform.localPosition = ballHoldingPosition;

        }

        public override void MoveBall()
        {
            //Do nothing
        }
    }
}
