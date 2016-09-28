using UnityEngine;

public class BlockInput : AbilityInput
{
    public override bool Activated()
    {
        return Input.GetKeyDown(KeyCode.R);
    }
}
