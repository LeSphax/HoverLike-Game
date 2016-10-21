using UnityEngine;

public class BrakeInput : AbilityInput
{
    public override bool Activated()
    {
        return Input.GetKey(KeyCode.Z);
    }

    public override string GetKey()
    {
        return "Z";
    }
}
