using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PersistentAbilityEffect : AbilityEffect
{
    protected PlayerPhysicsModel model;

    public abstract void Simulate(float dt);

    public override void ApplyEffect(PlayerPhysicsModel model)
    {
        model.AddPersistentEffect(this);
        this.model = model;
    }

    public override void UnApplyEffect(PlayerPhysicsModel playerPhysicsModel)
    {
        playerPhysicsModel.RemovePersistentEffect(this);
    }
}
