using UnityEngine;

//Apply an effect using parameters given by the corresponding abilityTargeting.
public abstract class AbilityEffect : MonoBehaviour
{
    public virtual void ApplyOnTarget(params object[] parameters)
    {
        ActualAbilitiesLatency.commandsSent.AddInQueue(this.GetType(), Time.realtimeSinceStartup * 1000);
    }
}