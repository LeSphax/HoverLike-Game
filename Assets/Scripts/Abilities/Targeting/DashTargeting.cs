using PlayerManagement;
using UnityEngine;

public class DashTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        Vector3 position = MyComponents.MyPlayer.InputManager.GetMouseLocalPosition();
        callback.Invoke(true, MyComponents.MyPlayer.controller, position);
    }
}
