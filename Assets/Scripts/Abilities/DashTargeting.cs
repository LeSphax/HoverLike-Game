using PlayerManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DashTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        Vector3 position = Functions.GetMouseWorldPosition();
        callback.Invoke(Players.MyPlayer.gameobjectAvatar,position);
    }
}
