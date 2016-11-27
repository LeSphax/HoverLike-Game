using UnityEngine;
using System.Collections;
using PlayerManagement;

public class TeleportTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        callback.Invoke();
    }
}
