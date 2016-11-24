using PlayerManagement;
using UnityEngine;

public class JumpTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        callback.Invoke(Players.MyPlayer.controller);
    }
}
