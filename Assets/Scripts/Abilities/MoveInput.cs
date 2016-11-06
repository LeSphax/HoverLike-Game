using UnityEngine;

public class MoveInput : AbilityInput
{
    protected override bool FirstActivation()
    {
        return Input.GetMouseButton(1);
    }

    public override bool HasIcon()
    {
        return false;
    }
}
