using PlayerManagement;
using UnityEngine;

public class ShootInput : AbilityInputWithBall
{
    public override bool FirstActivation()
    {
        return MyComponents.InputManager.GetMouseButtonDown(0) && !AbilityTargeting.IsTargeting;
    }

    public override bool ContinuousActivation()
    {
        return MyComponents.InputManager.GetMouseButton(0) && !AbilityTargeting.IsTargeting;
    }

    public override bool SecondActivation()
    {
        return MyComponents.InputManager.GetMouseButtonUp(0);
    }

    public override bool Cancellation()
    {
        return MyComponents.InputManager.GetMouseButtonDown(1);
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
