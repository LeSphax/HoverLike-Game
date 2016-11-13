using System;
using PlayerManagement;
using UnityEngine;

public class PassEffect : AbilityEffectBuilder
{

    public void ApplyOnTarget(GameObject targetGameObject, Vector3 targetPosition)
    {
        MyComponents.NetworkViewsManagement.InstantiateOnServer("Effects/PassManager", Vector3.zero, Quaternion.identity, Players.myPlayerId, targetPosition);
    }

    public override AbilityEffect GetEffect(params object[] parameters)
    {
        throw new NotImplementedException();
    }
}
