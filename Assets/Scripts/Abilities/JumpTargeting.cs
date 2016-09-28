using UnityEngine;

public class JumpTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        GameObject target = GameObjects.MyPlayer();
        callback.Invoke(target, target.transform.position);
    }
}
