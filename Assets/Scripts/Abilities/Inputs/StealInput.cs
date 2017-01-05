using UnityEngine;

public class StealInput : AbilityInput
{

    private const int INPUT_NUMBER = 4;

    public override bool FirstActivation()
    {
        return Input.GetKeyDown(UserSettings.GetKeyCode(INPUT_NUMBER));
    }

    public override string GetKey()
    {
        return UserSettings.GetKeyForIcon(INPUT_NUMBER);
    }
}
