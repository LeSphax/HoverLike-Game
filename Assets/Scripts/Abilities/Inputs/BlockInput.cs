using System;
using UnityEngine;

public class BlockInput : AbilityInput
{
    private const int INPUT_NUMBER = 1;

    protected override bool FirstActivation()
    {
        return Input.GetKeyDown(Inputs.GetKeyCode(INPUT_NUMBER));
    }

    public override string GetKey()
    {
        return Inputs.GetKeyForIcon(INPUT_NUMBER);
    }
}
