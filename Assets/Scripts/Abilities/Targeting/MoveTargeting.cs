public class MoveTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        if  (MyComponents.Players.players[PlayerId] != null)
        {
            callback.Invoke(true, MyComponents.Players.players[PlayerId].controller, MyComponents.Players.players[PlayerId].InputManager.GetInputDirection());
        }
    }
}
