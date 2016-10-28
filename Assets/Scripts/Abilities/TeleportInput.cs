using UnityEngine;

public class TeleportInput : AbilityInput
{
    protected override bool FirstActivation()
    {
        return Input.GetKeyDown(KeyCode.R);
    }

    public override string GetKey()
    {
        return "R";
    }
}
