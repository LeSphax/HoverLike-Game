using AbilitiesManagement;
using PlayerManagement;
using UnityEngine;

public class TeleportEffect : AbilityEffect
{

    public override void ApplyOnTarget(params object[] parameters)
    {
        Players.MyPlayer.controller.View.RPC("Teleport", RPCTargets.Server);
    }
}

public class TeleportPersistentEffect : PersistentEffect
{
    private float TimeBeforeTeleportation = 1.5f;

    public TeleportPersistentEffect(AbilitiesManager manager) : base(manager)
    {
        duration = TimeBeforeTeleportation;
        manager.controller.StopMoving();
        manager.controller.GetComponent<Rigidbody>().isKinematic = true;
    }

    protected override void Apply(float dt)
    {

    }

    public override void StopEffect()
    {
        manager.transform.position = manager.controller.Player.SpawningPoint;
        manager.controller.GetComponent<Rigidbody>().isKinematic = false;
    }
}