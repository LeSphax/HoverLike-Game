using System;
using UnityEngine;

public class BlockInput : AbilityInput
{
    public override bool Activated()
    {
        return Input.GetKeyDown(KeyCode.R);
    }

    public override string GetKey()
    {
        return "R";
    }
}
