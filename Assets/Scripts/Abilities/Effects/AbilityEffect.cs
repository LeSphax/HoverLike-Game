using UnityEngine;

//Apply an effect using parameters given by the corresponding abilityTargeting.
public abstract class AbilityEffect : SlideBall.MonoBehaviour
{
    public virtual void ApplyOnTarget(params object[] parameters)
    {
        if (!NetworkingState.IsServer)
            ActualAbilitiesLatency.commandsSent.AddInQueue(this.GetType(), Time.realtimeSinceStartup * 1000);
    }

    public virtual void ApplyOnTargetCancel(params object[] parameters)
    {
    }

    public virtual bool LongEffect
    {
        get
        {
            return false;
        }
    }
}