using UnityEngine;

public class StealTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        callback.Invoke(GameObjects.MyPlayer(),Vector3.zero);
    }
}
