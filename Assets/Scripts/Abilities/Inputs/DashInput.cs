using UnityEngine;

public class DashInput : AbilityInput
{

    private const int INPUT_NUMBER = 0;


    protected override bool FirstActivation()
    {
        return Input.GetKeyDown(Inputs.GetKeyCode(INPUT_NUMBER));
    }

    public override string GetKey()
    {
        return Inputs.GetKeyForIcon(INPUT_NUMBER);
    }
}
