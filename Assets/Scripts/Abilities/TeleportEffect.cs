using AbilitiesManagement;
using UnityEngine;

public class TeleportEffect : AbilityEffect
{

    public override void ApplyOnTarget(GameObject target, Vector3 position)
    {
        target.GetComponent<PlayerController>().View.RPC("Teleport", RPCTargets.Server);
    }
}

public class TeleportPersistentEffect : PersistentEffect
{
    private float TimeBeforeTeleportation = 1.5f;

    public TeleportPersistentEffect(AbilitiesManager manager) : base(manager)
    {
        duration = TimeBeforeTeleportation;
        manager.controller.StopMoving();
    }

    protected override void Apply(float dt)
    {
    }

    protected override void StopEffect()
    {
        manager.transform.position = manager.controller.Player.SpawningPoint;
    }
}