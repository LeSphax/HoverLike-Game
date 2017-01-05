using UnityEngine;

public class PassInput : AbilityInputWithBall
{

    private const int INPUT_NUMBER = 3;

    public override bool FirstActivation()
    {
        return Input.GetKeyDown(UserSettings.GetKeyCode(INPUT_NUMBER));
    }

    public override bool SecondActivation()
    {
        return Input.GetMouseButtonDown(0);
    }

    public override bool Cancellation()
    {
        return Input.GetMouseButtonDown(1);
    }

    public override string GetKey()
    {
        return UserSettings.GetKeyForIcon(INPUT_NUMBER);
    }

}
