using UnityEngine;

public class BlockTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        GameObject target = MyGameObjects.MyPlayer();
        callback.Invoke(target,target.transform.position);
    }
}
