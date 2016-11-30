using UnityEngine;

public class MoveInput : AbilityInput
{

    protected override bool FirstActivation()
    {
        return Input.GetMouseButtonDown(1);
    }

    public override bool HasIcon()
    {
        return false;
    }

    protected override bool IsMovement()
    {
        return true;
    }
}
