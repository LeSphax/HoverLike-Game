using UnityEngine;

public class StealTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        GameObject target = GameObjects.MyPlayer();
        callback.Invoke(target, target.transform.position);
    }
}
