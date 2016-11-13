using UnityEngine;
using System.Collections;
using PlayerManagement;
using System.Collections.Generic;

public class TeleportTargeting : AbilityTargeting
{
    public override List<AbilityEffect> StartTargeting(CastOnTarget callback)
    {
        return callback.Invoke(Players.MyPlayer.physicsModel);
    }
}
