using UnityEngine;

public class JumpTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        callback.Invoke(GameObjects.MyPlayer(),Vector3.zero);
    }
}
