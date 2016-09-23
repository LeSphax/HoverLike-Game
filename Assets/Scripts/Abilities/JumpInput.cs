using UnityEngine;

public class JumpInput : AbilityInput
{
    public override bool Activated()
    {
        return Input.GetKeyDown(KeyCode.E);
    }
}
