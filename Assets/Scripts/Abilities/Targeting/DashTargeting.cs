using UnityEngine;

public class DashTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        Vector3 position = MyComponents.Players.players[PlayerId].InputManager.GetMouseWorldPosition();
        callback.Invoke(true, MyComponents.Players.players[PlayerId].controller, position);
    }
}
