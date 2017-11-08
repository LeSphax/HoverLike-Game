using UnityEngine;

public class MoveInput : AbilityInput
{

    public static bool ContinuousMovement = true;

    public override bool FirstActivation()
    {
        return true;// Functions.GetInputDirection() != Vector3.zero;

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
