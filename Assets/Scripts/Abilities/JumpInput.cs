using UnityEngine;

public class JumpInput : AbilityInput
{
    protected override bool FirstActivation()
    {
        return Input.GetKeyDown(KeyCode.Z);
    }

    public override string GetKey()
    {
        return "Z";
    }
}
