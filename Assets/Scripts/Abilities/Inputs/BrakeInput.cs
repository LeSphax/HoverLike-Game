using PlayerManagement;
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
        bool result = SlideBallInputs.GetKey(UserSettings.GetKeyCode(INPUT_NUMBER), SlideBallInputs.GUIPart.ABILITY);
        if (result)
            Players.MyPlayer.controller.abilitiesManager.EffectsManager.ActivateSlow(true);
        return result;
    }

    public override string GetKeyForGUI()
    {
        return UserSettings.GetKeyForIcon(INPUT_NUMBER);
    }

    public override bool Cancellation()
    {
        bool result = Input.GetKeyUp(UserSettings.GetKeyCode(INPUT_NUMBER));
        if (result)
            Players.MyPlayer.controller.abilitiesManager.EffectsManager.ActivateSlow(false);
        return result;
    }

    protected override bool IsMovement()
    {
        return true;
    }
}
