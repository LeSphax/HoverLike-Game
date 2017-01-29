using PlayerManagement;

public class StealTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        callback.Invoke(true,Players.MyPlayer.controller);
    }
}
