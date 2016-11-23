using System;
using UnityEngine;

public class BlockInput : AbilityInput
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
