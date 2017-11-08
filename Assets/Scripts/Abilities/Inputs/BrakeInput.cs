using UnityEngine;

public class BrakeInput : AbilityInput
{
    protected override int INPUT_NUMBER
    {
        get
        {
            return 1;
        }
    }

    public override bool FirstActivation()
    {
        return false; // SlideBallInputs.GetKey(UserSettings.GetKeyCode(INPUT_NUMBER), SlideBallInputs.GUIPart.ABILITY);
    }

    public override string GetKeyForGUI()
    {
        return UserSettings.GetKeyForIcon(INPUT_NUMBER);
    }

    public override bool Cancellation()
    {
        return Input.GetKeyUp(UserSettings.KeyForInputCheck(INPUT_NUMBER));
    }

    protected override bool IsMovement()
    {
        return true;
    }
}
