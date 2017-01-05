using UnityEngine;

public class JumpInput : AbilityInput
{

    private const int INPUT_NUMBER = 2;

    public override bool FirstActivation()
    {
        return Input.GetKeyDown(UserSettings.GetKeyCode(INPUT_NUMBER));
    }

    public override string GetKey()
    {
        return UserSettings.GetKeyForIcon(INPUT_NUMBER);
    }

    protected override bool IsMovement()
    {
        return true;
    }
}
