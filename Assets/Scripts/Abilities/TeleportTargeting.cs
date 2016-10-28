using UnityEngine;
using System.Collections;
using PlayerManagement;

public class TeleportTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        GameObject target = MyGameObjects.MyPlayer();
        callback.Invoke(target, Players.MyPlayer.SpawningPoint);
    }
}
