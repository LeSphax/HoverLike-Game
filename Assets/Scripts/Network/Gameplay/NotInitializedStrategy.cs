using System;

public class NotInitializedStrategy : PlayerMovementStrategy
{
    public override float MaxPlayerVelocity
    {
        get
        {
            return 0;
        }
    }

    protected override void Move()
    {
        //DoNothing
    }

    protected override void StopMoving()
    {
        //DoNothing
    }
}
