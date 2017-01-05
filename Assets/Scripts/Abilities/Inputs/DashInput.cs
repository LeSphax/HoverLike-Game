using UnityEngine;

public class DashInput : AbilityInput
{

    private const int INPUT_NUMBER = 0;


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
