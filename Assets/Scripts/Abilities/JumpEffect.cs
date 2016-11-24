using UnityEngine;

public class JumpEffect : AbilityEffect
{
    public override void ApplyOnTarget(params object[] parameters)
    {
        PlayerController controller = (PlayerController)parameters[0];
        controller.movementManager.View.RPC("Jump", RPCTargets.Server);
    }
}
