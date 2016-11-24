using PlayerManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BrakeTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        callback.Invoke(Players.MyPlayer.controller);
    }
}
