using UnityEngine;

public class PassInput : AbilityInputWithBall
{

    protected override int INPUT_NUMBER
    {
        get
        {
            return 3;
        }
    }

    public override bool SecondActivation()
    {
        return Input.GetMouseButtonDown(0);
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
