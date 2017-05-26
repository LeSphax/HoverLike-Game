public class BrakeEffect : AbilityEffect
{

    public override bool LongEffect
    {
        get
        {
            return true;
        }
    }

    public override void ApplyOnTarget(params object[] parameters)
    {
        Brake(true, parameters);
    }

    public override void ApplyOnTargetCancel(params object[] parameters)
    {
        Brake(false, parameters);
    }

    private void Brake(bool activate, params object[] parameters)
    {
        PlayerController controller = (PlayerController)parameters[0];
        controller.abilitiesManager.View.RPC("Brake", RPCTargets.Server, activate);
        controller.abilitiesManager.EffectsManager.ActivateSlow(activate);
    }

}
