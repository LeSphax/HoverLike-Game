using UnityEngine;

public class JumpInput : AbilityInput
{

    private const int INPUT_NUMBER = 2;

    protected override bool FirstActivation()
    {
        return Input.GetKeyDown(Inputs.GetKeyCode(INPUT_NUMBER));
    }

    public override string GetKey()
    {
        return Inputs.GetKeyForIcon(INPUT_NUMBER);
    }
}
