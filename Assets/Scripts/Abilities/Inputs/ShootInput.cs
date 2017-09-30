using PlayerManagement;
using UnityEngine;

public class ShootInput : AbilityInputWithBall
{
    public override bool FirstActivation()
    {
        return Input.GetMouseButtonDown(0) && !AbilityTargeting.IsTargeting;
    }

    public override bool ContinuousActivation()
    {
        return Input.GetMouseButton(0) && !AbilityTargeting.IsTargeting;
    }

    public override bool SecondActivation()
    {
        return Input.GetMouseButtonUp(0);
    }

    public override bool Cancellation()
    {
        return Input.GetMouseButtonDown(1);
    }

    public override bool HasIcon()
    {
        return false;
    }

    public override bool HasErrorSound()
    {
        return false;
    }
}
