using UnityEngine;

public class JumpEffect : AbilityEffect
{
    public override void ApplyOnTarget(GameObject target, Vector3 position)
    {
        target.GetComponent<PlayerController>().movementManager.View.RPC("Jump", RPCTargets.Server);
    }
}
