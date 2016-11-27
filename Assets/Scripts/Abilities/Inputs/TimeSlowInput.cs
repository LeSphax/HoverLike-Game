using UnityEngine;

public class TimeSlowInput : AbilityInput
{

    private const int INPUT_NUMBER = 2;

    protected override bool FirstActivation()
    {
        return Input.GetKeyDown(Inputs.GetKeyCode(INPUT_NUMBER));
    }

    protected override bool Cancellation()
    {
        return Input.GetMouseButtonDown(1);
    }

    public override string GetKey()
    {
        return Inputs.GetKeyForIcon(INPUT_NUMBER);
    }

}
