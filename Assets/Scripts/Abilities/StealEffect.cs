using AbilitiesManagement;
using UnityEngine;

public class StealEffect : AbilityEffect
{

    public float stealingDuration = 0.5f;

    public override void ApplyOnTarget(GameObject target, Vector3 position)
    {
        target.GetComponent<PlayerController>().abilitiesManager.View.RPC("Steal",RPCTargets.Server,stealingDuration);
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

    protected override void StopEffect()
    {
        manager.controller.ballController.Stealing = false;
    }

}
