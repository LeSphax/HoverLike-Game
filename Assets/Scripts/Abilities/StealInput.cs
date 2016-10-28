using UnityEngine;

public class StealInput : AbilityInput
{
    protected override bool FirstActivation()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public override string GetKey()
    {
        return "SPC";
    }
}
