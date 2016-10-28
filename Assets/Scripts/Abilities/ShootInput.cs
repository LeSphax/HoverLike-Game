using System;
using UnityEngine;

public class ShootInput : AbilityInput
{
    protected override bool FirstActivation()
    {
        return Input.GetMouseButtonDown(0);
    }

    protected override bool SecondActivation()
    {
        return Input.GetMouseButtonUp(0);
    }

    protected override bool Cancellation()
    {
        return Input.GetMouseButtonDown(1);
    }

    protected override bool HasIcon()
    {
        return false;
    }
}
