using UnityEngine;

public class ShootInput : AbilityInputWithBall
{
    protected override bool FirstActivation()
    {
        return Input.GetMouseButtonDown(0) && !AbilityTargeting.IsTargeting;
    }

    protected override bool SecondActivation()
    {
        return Input.GetMouseButtonUp(0);
    }

    protected override bool Cancellation()
    {
        return Input.GetMouseButtonDown(1);
    }

    public override bool HasIcon()
    {
        return false;
    }
}
