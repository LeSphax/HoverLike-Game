using PlayerManagement;
using UnityEngine;

public class MoveTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        if  (MyComponents.MyPlayer != null)
        {
            
            callback.Invoke(true, MyComponents.MyPlayer.controller, MyComponents.MyPlayer.InputManager.GetInputDirection());
        }
    }
}
