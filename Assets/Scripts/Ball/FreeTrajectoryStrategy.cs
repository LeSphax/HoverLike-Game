namespace Ball
{
    public class FreeTrajectoryStrategy : BallTrajectoryStrategy
    {

        public FreeTrajectoryStrategy()
        {
            if (MyComponents.NetworkManagement.isServer)
            {
                AttractionBall.Activated = true;
                MyComponents.BallState.UnCatchable = false;
                MyComponents.BallState.DetachBall();
                MyComponents.BallState.TrySetKinematic();
            }
            //MyComponents.BallState.protectionSphere.SetActive(false);
            MyComponents.BallState.transform.SetParent(null);
        }

        public override void MoveBall()
        {
            //Do nothing
        }
    }
}
