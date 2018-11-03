using PlayerManagement;
using UnityEngine;

public class ShootInput : AbilityInputWithBall
{
    public override bool FirstActivation()
    {
        return MyComponents.MyPlayer.InputManager.GetMouseButtonDown(0) && !AbilityTargeting.IsTargeting;
    }

    public override bool ContinuousActivation()
    {
        return MyComponents.MyPlayer.InputManager.GetMouseButton(0) && !AbilityTargeting.IsTargeting;
    }

    public override bool SecondActivation()
    {
        return MyComponents.MyPlayer.InputManager.GetMouseButtonUp(0);
    }

    public override bool Cancellation()
    {
        return MyComponents.MyPlayer.InputManager.GetMouseButtonDown(1);
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
