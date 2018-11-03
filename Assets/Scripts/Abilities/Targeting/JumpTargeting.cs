using PlayerManagement;

public class JumpTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        callback.Invoke(true, MyComponents.MyPlayer.controller);
    }
}
