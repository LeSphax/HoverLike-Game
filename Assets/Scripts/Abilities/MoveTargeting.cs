using PlayerManagement;
using System.Collections.Generic;
using UnityEngine;

public class MoveTargeting : AbilityTargeting
{
    public override List<AbilityEffect> StartTargeting(CastOnTarget callback)
    {
        Vector3 position = Functions.GetMouseWorldPosition();
        return callback.Invoke(position);
    }
}
