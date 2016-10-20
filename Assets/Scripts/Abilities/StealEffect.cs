using UnityEngine;

public class StealEffect : AbilityEffect
{

    public float stealingDuration = 0.5f;
    private GameObject target;

    public override void ApplyOnTarget(GameObject target, Vector3 position)
    {
        this.target = target;
        target.GetComponent<PlayerBallController>().Stealing = true;
        Invoke("StopStealing", stealingDuration);
    }                   

    private void StopStealing()
    {
        target.GetComponent<PlayerBallController>().Stealing = false;
    }
}
