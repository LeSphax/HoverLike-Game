using UnityEngine;

public class TimeSlowInput : AbilityInput
{

    protected override int INPUT_NUMBER
    {
        get
        {
            return 2;
        }
    }

    public override bool Cancellation()
    {
        return Input.GetMouseButtonDown(1);
    }

    public override string GetKeyForGUI()
    {
        return UserSettings.GetKeyForIcon(INPUT_NUMBER);
    }

}
