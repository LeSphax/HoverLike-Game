using UnityEngine;

public class BrakeEffect : AbilityEffect
{

    public override void ApplyOnTarget(GameObject target, Vector3 position)
    {
        target.GetComponent<PlayerController>().movementManager.View.RPC("Brake", RPCTargets.Server);
    }
}
