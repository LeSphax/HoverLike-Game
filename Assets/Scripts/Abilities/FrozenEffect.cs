using PlayerManagement;
using System;
using UnityEngine;

public class FrozenEffect : AbilityEffectBuilder
{
    public float duration;

    public void ApplyOnTarget(GameObject target, Vector3 position)
    {
        Players.MyPlayer.CurrentState = Player.State.FROZEN;
        Invoke("StopFreezing", duration);
    }

    public override AbilityEffect GetEffect(params object[] parameters)
    {
        throw new NotImplementedException();
    }

    private void StopFreezing()
    {
        if (Players.MyPlayer.CurrentState == Player.State.FROZEN)
            Players.MyPlayer.CurrentState = Player.State.PLAYING;
    }
}
