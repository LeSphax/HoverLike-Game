using PlayerManagement;
using UnityEngine;

public class BlockEffect : AbilityEffect
{

    public override void ApplyOnTarget(GameObject target, Vector3 position)
    {
        MyComponents.NetworkViewsManagement.Instantiate("Effects/BlockExplosion", Players.myPlayerId);
    }

}
