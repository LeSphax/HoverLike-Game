using System;
using UnityEngine;

public class BlockInput : AbilityInput
{
    private const int INPUT_NUMBER = 1;

    public override bool FirstActivation()
    {
        return Input.GetKeyDown(UserSettings.GetKeyCode(INPUT_NUMBER));
    }

    public override string GetKey()
    {
        return UserSettings.GetKeyForIcon(INPUT_NUMBER);
    }
}
