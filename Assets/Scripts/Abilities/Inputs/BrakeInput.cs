using PlayerManagement;
using UnityEngine;

public class BrakeInput : AbilityInput
{
    private const int INPUT_NUMBER = 1;

    public override bool FirstActivation()
    {
        bool result = Input.GetKey(UserSettings.GetKeyCode(INPUT_NUMBER));
        if (result)
            Players.MyPlayer.controller.abilitiesManager.EffectsManager.ActivateSlow(true);
        return result;
    }

    public override string GetKey()
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
