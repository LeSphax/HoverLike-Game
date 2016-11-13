using PlayerManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BrakeTargeting : AbilityTargeting
{
    public override List<AbilityEffect> StartTargeting(CastOnTarget callback)
    {
        return callback.Invoke(Players.MyPlayer.physicsModel);
    }
}
