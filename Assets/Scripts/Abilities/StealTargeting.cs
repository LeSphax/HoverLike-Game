using UnityEngine;

public class StealTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        GameObject target = MyGameObjects.MyPlayer();
        callback.Invoke(target, target.transform.position);
    }
}
