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
        return false; // MyComponents.Players.players[playerId].InputManager.GetKey(UserSettings.GetKeyCode(INPUT_NUMBER), GUIPart.ABILITY);
    }

    public override string GetKeyForGUI()
    {
        return UserSettings.GetKeyForIcon(INPUT_NUMBER);
    }

    public override bool Cancellation()
    {
        return MyComponents.Players.players[PlayerId].InputManager.GetKeyUp(UserSettings.GetKeyCode(INPUT_NUMBER));
    }

    protected override bool IsMovement()
    {
        return true;
    }
}
