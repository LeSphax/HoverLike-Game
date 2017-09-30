using UnityEngine;

public class JumpInput : AbilityInput
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

    protected override bool IsMovement()
    {
        return true;
    }
}
