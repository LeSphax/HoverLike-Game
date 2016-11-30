using UnityEngine;

public class BrakeInput : AbilityInput
{
    private const int INPUT_NUMBER = 1;

    protected override bool FirstActivation()
    {
        return Input.GetKey(Inputs.GetKeyCode(INPUT_NUMBER));
    }

    public override string GetKey()
    {
        return Inputs.GetKeyForIcon(INPUT_NUMBER);
    }

    protected override bool IsMovement()
    {
        return true;
    }
}
