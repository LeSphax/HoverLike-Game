using PlayerManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BrakeTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        GameObject target = Players.MyPlayer.gameobjectAvatar;
        callback.Invoke(target, target.transform.position);
    }
}
