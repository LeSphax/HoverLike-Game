using PlayerManagement;
using UnityEngine;

public class BlockTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        callback.Invoke();
    }
}
