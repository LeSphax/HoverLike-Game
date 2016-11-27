using PlayerManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FrozenEffect : AbilityEffect
{
    public float duration;

    public override void ApplyOnTarget(params object[] parameters)
    {
        Players.MyPlayer.CurrentState = Player.State.FROZEN;
        Invoke("StopFreezing", duration);
    }

    private void StopFreezing()
    {
        if (Players.MyPlayer.CurrentState == Player.State.FROZEN)
            Players.MyPlayer.CurrentState = Player.State.PLAYING;
    }
}
