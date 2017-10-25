using AbilitiesManagement;

public class StealEffect : AbilityEffect
{

    public static float stealingDuration = 0.5f;
    public static float protectionDuration = 0.3f;

    public override void ApplyOnTarget(params object[] parameters)
    {
        base.ApplyOnTarget(parameters);
        PlayerController controller = (PlayerController)parameters[0];
        controller.abilitiesManager.View.RPC("Steal", RPCTargets.Server);
    }

}

public class StealPersistentEffect : PersistentEffect
{

    public StealPersistentEffect(AbilitiesManager manager) : base(manager)
    {
        manager.controller.Player.State.Stealing = PlayerManagement.StealingState.STEALING;
        this.duration = StealEffect.stealingDuration;
    }


    public StealPersistentEffect(AbilitiesManager manager, float duration) : base(manager)
    {
        manager.controller.Player.State.Stealing = PlayerManagement.StealingState.STEALING;
        this.duration = duration;
    }


    protected override void Apply(float dt)
    {
    }

    public override void StopEffect()
    {
        manager.controller.Player.State.Stealing = PlayerManagement.StealingState.IDLE;
    }

}

public class ProtectionPersistentEffect : PersistentEffect
{

    public ProtectionPersistentEffect(AbilitiesManager manager) : base(manager)
    {
        manager.controller.Player.State.Stealing = PlayerManagement.StealingState.PROTECTED;
        this.duration = StealEffect.protectionDuration;
    }

    protected override void Apply(float dt)
    {
    }

    public override void StopEffect()
    {
        manager.controller.Player.State.Stealing = PlayerManagement.StealingState.IDLE;
    }

}
