using PlayerManagement;
using System;
using UnityEngine;

public class FrozenEffectBuilder : AbilityEffectBuilder
{
    public float duration;

    public void ApplyOnTarget(GameObject target, Vector3 position)
    {
        Players.MyPlayer.CurrentState = Player.State.FROZEN;
        Invoke("StopFreezing", duration);
    }

    public override AbilityEffect GetEffect(params object[] parameters)
    {
        return new FrozenEffect();
    }

    private void StopFreezing()
    {
        if (Players.MyPlayer.CurrentState == Player.State.FROZEN)
            Players.MyPlayer.CurrentState = Player.State.PLAYING;
    }
}

public class FrozenEffect : AbilityEffect
{
    public override void ApplyEffect(PlayerPhysicsModel model)
    {
        Debug.Log("FrozenEffect not implemented");
    }

    public override void UnApplyEffect(PlayerPhysicsModel playerPhysicsModel)
    {

    }
    public override bool IsSerialisable()
    {
        return false;
    }
}
