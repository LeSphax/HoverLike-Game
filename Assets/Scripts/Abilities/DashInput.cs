using UnityEngine;

public class DashInput : AbilityInput
{
    protected override bool FirstActivation()
    {
        return Input.GetKeyDown(KeyCode.A);
    }

    public override string GetKey()
    {
        return "A";
    }
}
