using System;
using UnityEngine;

public class BlockInput : AbilityInput
{
    protected override int INPUT_NUMBER
    {
        get
        {
            return 1;
        }
    }

    public override string GetKeyForGUI()
    {
        return UserSettings.GetKeyForIcon(INPUT_NUMBER);
    }
}
