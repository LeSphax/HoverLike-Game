namespace Ball
{
    public class FreeTrajectoryStrategy : BallTrajectoryStrategy
    {

        public FreeTrajectoryStrategy(BallState ballState) : base(ballState)
        {
            if (NetworkingState.IsServer)
            {
                AttractionBall.Activated = true;
                BallState.UnCatchable = false;
                BallState.DetachBall();
                BallState.TrySetKinematic();
            }
            //BallState.protectionSphere.SetActive(false);
            BallState.SetWorldAsParent();
        }

        public override void MoveBall()
        {
            //Do nothing
        }
    }
}
