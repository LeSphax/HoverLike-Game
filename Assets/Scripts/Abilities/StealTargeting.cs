using PlayerManagement;
using UnityEngine;

public class StealTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        callback.Invoke(Players.MyPlayer.controller);
    }
}
