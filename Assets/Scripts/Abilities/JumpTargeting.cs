using PlayerManagement;
using System.Collections.Generic;
using UnityEngine;

public class JumpTargeting : AbilityTargeting
{
    public override List<AbilityEffect> StartTargeting(CastOnTarget callback)
    {
        return callback.Invoke(Players.MyPlayer.physicsModel);
    }
}
