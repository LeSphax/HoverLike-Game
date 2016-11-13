using System.Collections.Generic;
using UnityEngine;

public delegate List<AbilityEffect> CastOnTarget(params object[] parameters);

public abstract class AbilityTargeting : MonoBehaviour
{

    public static bool IsTargeting;

    public abstract List<AbilityEffect> StartTargeting(CastOnTarget callback);


    public virtual List<AbilityEffect> ReactivateTargeting()
    {
        return null;
    }

    public virtual void CancelTargeting()
    {

    }
}