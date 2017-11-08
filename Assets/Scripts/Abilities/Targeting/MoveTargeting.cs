using PlayerManagement;
using UnityEngine;

public class MoveTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        if (Players.MyPlayer != null)
        {
            
            callback.Invoke(true, Players.MyPlayer.controller, Functions.GetInputDirection());
        }
    }
}
