using System;
using PlayerManagement;
using UnityEngine;

public class BlockEffect : AbilityEffectBuilder
{

    //public override void ApplyOnTarget(Phys target, Vector3 position)
    //{
    //    MyComponents.NetworkViewsManagement.Instantiate("Effects/BlockExplosion", Players.myPlayerId);
    //}
    public override AbilityEffect GetEffect(params object[] parameters)
    {
        throw new NotImplementedException();
    }
}
