using UnityEngine;

public class JumpInput : AbilityInput
{
    protected override bool FirstActivation()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    public override string GetKey()
    {
        return "E";
    }
}
