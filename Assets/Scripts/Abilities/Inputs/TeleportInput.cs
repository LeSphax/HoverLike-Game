using UnityEngine;

public class TeleportInput : AbilityInput
{

    protected override int INPUT_NUMBER
    {
        get
        {
            return 3;
        }
    }

    public override string GetKeyForGUI()
    {
        return UserSettings.GetKeyForIcon(INPUT_NUMBER);
    }

    protected override bool IsMovement()
    {
        return true;
    }
}
