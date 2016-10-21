using UnityEngine;

public class StealInput : AbilityInput
{
    public override bool Activated()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public override string GetKey()
    {
        return "SPC";
    }
}
