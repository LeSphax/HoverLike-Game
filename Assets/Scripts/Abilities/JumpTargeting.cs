using PlayerManagement;
using UnityEngine;

public class JumpTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        GameObject target = Players.MyPlayer.gameobjectAvatar;
        callback.Invoke(target, target.transform.position);
    }
}
