using AbilitiesManagement;
using UnityEngine;

public class StealEffect : AbilityEffect
{

    public float stealingDuration = 0.5f;

    public override void ApplyOnTarget(params object[] parameters)
    {
        PlayerController controller = (PlayerController)parameters[0];
        controller.abilitiesManager.View.RPC("Steal", RPCTargets.Server, stealingDuration);
    }

}

public class StealPersistentEffect : PersistentEffect
{

    public StealPersistentEffect(AbilitiesManager manager, float duration) : base(manager)
    {
        manager.controller.ballController.Stealing = true;
        this.duration = duration;
    }

    protected override void Apply(float dt)
    {
        manager.controller.ballController.Stealing = true;
    }

    public override void StopEffect()
    {
        manager.controller.ballController.Stealing = false;
    }

}
