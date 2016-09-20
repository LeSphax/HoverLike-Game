using UnityEngine;

public class BrakeInput : AbilityInput
{
    public override bool Activated()
    {
        return Input.GetKey(KeyCode.Z);
    }
}
