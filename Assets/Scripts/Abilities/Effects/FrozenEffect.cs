using PlayerManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FrozenEffect : AbilityEffect
{
    public float duration;

    public override void ApplyOnTarget(params object[] parameters)
    {
        Players.MyPlayer.State.Movement = MovementState.FROZEN;
        Invoke("StopFreezing", duration);
    }

    private void StopFreezing()
    {
        if (Players.MyPlayer.State.Movement == MovementState.FROZEN)
            Players.MyPlayer.State.Movement = MovementState.PLAYING;
    }
}
