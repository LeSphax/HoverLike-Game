using Byn.Net;

public delegate void CastOnTarget(bool cancelled, params object[] parameters);

//Shows the UI that allow the user to choose his target and call the CastOnTarget delegate with the results. 
//For example, choosing the target of a pass or the area to cast a spell on.
public abstract class AbilityTargeting : SlideBall.MonoBehaviour
{
    public ConnectionId PlayerId;

    public static bool IsTargeting;

    public abstract void ChooseTarget(CastOnTarget callback);

    public virtual void CancelTargeting()
    {

    }

    public virtual void ReactivateTargeting()
    {
    }
}