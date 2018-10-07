﻿using PlayerManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FrozenEffect : AbilityEffect
{
    public float duration;

    public override void ApplyOnTarget(params object[] parameters)
    {
        MyComponents.Players.MyPlayer.State.Movement = MovementState.FROZEN;
        Invoke("StopFreezing", duration);
    }

    private void StopFreezing()
    {
        if (MyComponents.Players.MyPlayer.State.Movement == MovementState.FROZEN)
            MyComponents.Players.MyPlayer.State.Movement = MovementState.PLAYING;
    }
}
