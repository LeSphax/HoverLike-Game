using PlayerManagement;
using UnityEngine;

public class StealTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        GameObject target = Players.MyPlayer.gameobjectAvatar;
        callback.Invoke(target, target.transform.position);
    }
}
