using System;
using UnityEngine;

public class BlockInput : AbilityInput
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
