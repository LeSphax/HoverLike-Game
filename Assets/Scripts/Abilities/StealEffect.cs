using System;
using PlayerBallControl;
using UnityEngine;

public class StealEffect : AbilityEffectBuilder
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
        throw new NotImplementedException();
    }

    private void StopStealing()
    {
        target.GetComponent<PlayerBallController>().Stealing = false;
    }
}
