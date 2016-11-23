using UnityEngine;

public class TimeSlowInput : AbilityInput
{
    protected override bool FirstActivation()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    protected override bool Cancellation()
    {
        return Input.GetMouseButtonDown(1);
    }

    public override string GetKey()
    {
        return "E";
    }

}
