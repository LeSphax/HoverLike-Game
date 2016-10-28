using UnityEngine;

public class BrakeInput : AbilityInput
{
    protected override bool FirstActivation()
    {
        return Input.GetKey(KeyCode.Z);
    }

    public override string GetKey()
    {
        return "Z";
    }
}
