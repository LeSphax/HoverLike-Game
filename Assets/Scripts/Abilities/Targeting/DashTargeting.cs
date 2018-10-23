using PlayerManagement;
using UnityEngine;

public class DashTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        Vector3 position = MyComponents.InputManager.GetMouseLocalPosition();
        callback.Invoke(true, MyComponents.Players.MyPlayer.controller, position);
    }
}
