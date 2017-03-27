using UnityEngine;

public class StealInput : AbilityInput
{

    protected override int INPUT_NUMBER
    {
        get
        {
            return 4;
        }
    }

    public override string GetKeyForGUI()
    {
        return UserSettings.GetKeyForIcon(INPUT_NUMBER);
    }

    public override bool HasErrorSound()
    {
        return false;
    }
}
