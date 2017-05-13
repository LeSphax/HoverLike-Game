using UnityEngine;

public class MoveInput : AbilityInput
{

    public override bool FirstActivation()
    {
        return Input.GetMouseButton(1);
    }

    public override bool HasIcon()
    {
        return false;
    }

    protected override bool IsMovement()
    {
        return true;
    }

    public override bool HasErrorSound()
    {
        return false;
    }
}
