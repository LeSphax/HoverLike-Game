using System;

public class NotInitializedStrategy : PlayerMovementStrategy
{
    protected override void Move()
    {
        //DoNothing
    }

    protected override void StopMoving()
    {
        //DoNothing
    }
}
