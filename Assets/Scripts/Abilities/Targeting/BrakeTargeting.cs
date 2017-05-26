using PlayerManagement;

public class BrakeTargeting: AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        callback.Invoke(true, Players.MyPlayer.controller);
    }
}

