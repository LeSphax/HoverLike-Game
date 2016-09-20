using UnityEngine;

public class DashInput : AbilityInput
{
    public override bool Activated()
    {
        return Input.GetKeyDown(KeyCode.A);
    }
}
