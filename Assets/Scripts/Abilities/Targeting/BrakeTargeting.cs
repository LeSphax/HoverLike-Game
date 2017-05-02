using PlayerManagement;

public class BrakeTargeting: AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        Players.MyPlayer.controller.abilitiesManager.View.RPC("Brake", RPCTargets.Server, true);
    }

    public override void CancelTargeting()
    {
        Players.MyPlayer.controller.abilitiesManager.View.RPC("Brake", RPCTargets.Server, false);
    }
}

