using UnityEngine;

public class MoveInput : AbilityInput
{
    protected override bool FirstActivation()
    {
        return Input.GetMouseButtonDown(1);
    }

    protected override bool HasIcon()
    {
        return false;
    }
}
