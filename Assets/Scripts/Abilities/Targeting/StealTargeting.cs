using PlayerManagement;

public class StealTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        callback.Invoke(true, MyComponents.Players.players[PlayerId].controller);
    }
}
