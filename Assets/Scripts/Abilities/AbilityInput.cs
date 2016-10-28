

using PlayerManagement;
using UnityEngine;
using UnityEngine.UI;

public abstract class AbilityInput : MonoBehaviour
{

    protected virtual bool HasIcon()
    {
        return true;
    }

    public bool Activate()
    {
        return FirstActivation() && CanBeActivated();
    }

    public bool Reactivate()
    {
        return SecondActivation() && CanBeActivated();
    }

    public bool Cancel()
    {
        return Cancellation() && CanBeActivated();
    }

    protected abstract bool FirstActivation();

    protected virtual bool SecondActivation()
    {
        return false;
    }

    protected virtual bool Cancellation()
    {
        return false;
    }

    protected virtual bool CanBeActivated()
    {
        return Players.MyPlayer.CurrentState == Player.State.PLAYING;
    }

    public virtual string GetKey()
    {
        return "";
    }



    void Start()
    {
        if (HasIcon())
        {
            GameObject keyToUsePrefab = Resources.Load<GameObject>("Prefabs/KeyToUse");
            GameObject keyToUse = Instantiate(keyToUsePrefab);
            keyToUse.transform.SetParent(transform, false);
            keyToUse.GetComponentInChildren<Text>().text = GetKey();
        }
    }
}