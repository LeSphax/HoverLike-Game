using PlayerManagement;
using UnityEngine;

public class MoveTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        if  (MyComponents.Players.MyPlayer != null)
        {
            
            callback.Invoke(true, MyComponents.Players.MyPlayer.controller, MyComponents.InputManager.GetInputDirection());
        }
    }
}
