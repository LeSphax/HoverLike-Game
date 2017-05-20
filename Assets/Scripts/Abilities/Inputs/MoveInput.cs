using UnityEngine;

public class MoveInput : AbilityInput
{

    public static bool ContinuousMovement = true;

    public override bool FirstActivation()
    {
        if (ContinuousMovement)
            return Input.GetMouseButton(1);
        else
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

    public override bool HasErrorSound()
    {
        return false;
    }
}
