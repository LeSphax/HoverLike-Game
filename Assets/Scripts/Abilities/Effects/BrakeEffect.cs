using UnityEngine;

public class BrakeEffect : AbilityEffect
{

    public override void ApplyOnTarget(params object[] parameters)
    {
        PlayerController controller = (PlayerController)parameters[0];
        controller.abilitiesManager.View.RPC("Brake", RPCTargets.Server);
    }
}
