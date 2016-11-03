using PlayerManagement;
using UnityEngine;

public class PassEffect : AbilityEffect
{

    public override void ApplyOnTarget(GameObject targetGameObject, Vector3 targetPosition)
    {
        MyComponents.NetworkViewsManagement.InstantiateOnServer("Effects/PassManager", Vector3.zero, Quaternion.identity, Players.myPlayerId, targetPosition);
    }

}
