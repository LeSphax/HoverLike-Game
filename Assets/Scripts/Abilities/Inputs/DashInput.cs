using UnityEngine;

public class DashInput : AbilityInput
{

    protected override int INPUT_NUMBER
    {
        get
        {
            return 0;
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
