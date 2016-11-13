using PlayerManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DashTargeting : AbilityTargeting
{
    public override List<AbilityEffect> StartTargeting(CastOnTarget callback)
    {
        Vector3 position = Functions.GetMouseWorldPosition();
        return callback.Invoke(Players.MyPlayer.physicsModel, position);
    }
}
