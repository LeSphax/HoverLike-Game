using UnityEngine;

public class TeleportInput : AbilityInput
{

    private const int INPUT_NUMBER = 3;

    protected override bool FirstActivation()
    {
        return Input.GetKeyDown(Inputs.GetKeyCode(INPUT_NUMBER));
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
