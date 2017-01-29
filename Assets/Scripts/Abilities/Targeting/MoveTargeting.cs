using PlayerManagement;
using UnityEngine;

public class MoveTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        Vector3 position = Functions.GetMouseWorldPosition();
        callback.Invoke(true, Players.MyPlayer.controller, position);
    }
}
