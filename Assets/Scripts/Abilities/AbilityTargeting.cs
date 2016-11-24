using UnityEngine;

public delegate void CastOnTarget(params object[] parameters);

public abstract class AbilityTargeting : MonoBehaviour
{

    public static bool IsTargeting;

    public abstract void ChooseTarget(CastOnTarget callback);

    public virtual void CancelTargeting()
    {

    }
}