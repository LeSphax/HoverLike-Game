using System;
using PlayerBallControl;
using UnityEngine;

public class StealEffectBuilder : AbilityEffectBuilder
{

    public float stealingDuration = 0.5f;
    private GameObject target;

    public  void ApplyOnTarget(GameObject target, Vector3 position)
    {
        this.target = target;
        target.GetComponent<PlayerBallController>().Stealing = true;
        Invoke("StopStealing", stealingDuration);
    }

    public override AbilityEffect GetEffect(params object[] parameters)
    {
        return new StealEffect();
    }

    private void StopStealing()
    {
        target.GetComponent<PlayerBallController>().Stealing = false;
    }
}

public class StealEffect : AbilityEffect
{
    public override void ApplyEffect(PlayerPhysicsModel model)
    {
        Debug.Log("Need to code steal effect");
    }

    public override int Deserialize(byte[] data, int currentIndex)
    {
        throw new NotImplementedException();
    }

    public override InputFlag GetInputFlag()
    {
        return InputFlag.SPACE;
    }


    public override bool IsSerialisable()
    {
        return false;
    }

    public override byte[] Serialize()
    {
        throw new NotImplementedException();
    }

    public override void UnApplyEffect(PlayerPhysicsModel playerPhysicsModel)
    {
    }
}
