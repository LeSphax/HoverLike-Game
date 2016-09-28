using UnityEngine;

public class BlockTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        GameObject target = GameObjects.MyPlayer();
        callback.Invoke(target,target.transform.position);
    }
}
