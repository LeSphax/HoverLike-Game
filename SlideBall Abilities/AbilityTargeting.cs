using System;
using UnityEngine;

public delegate void CastOnTarget(params object[] parameters);

//Shows the UI that allow the user to choose his target and call the CastOnTarget delegate with the results. 
//For example, choosing the target of a pass or the area to cast a spell on.
public abstract class AbilityTargeting : MonoBehaviour
{

    public static bool IsTargeting;

    public abstract void ChooseTarget(CastOnTarget callback);

    public virtual void CancelTargeting()
    {

    }

    public virtual void ReactivateTargeting()
    {
    }
}