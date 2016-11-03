using UnityEngine;

public class PassInput : AbilityInputWithBall
{
    protected override bool FirstActivation()
    {
        return Input.GetKeyDown(KeyCode.R);
    }

    protected override bool Cancellation()
    {
        return Input.GetMouseButtonDown(1);
    }

    public override string GetKey()
    {
        return "R";
    }
}
