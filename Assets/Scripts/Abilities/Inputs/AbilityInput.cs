

using PlayerManagement;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public delegate void ActivationHandler(bool activated);

public abstract class AbilityInput : MonoBehaviour
{
    protected bool StateIsPlaying;

    protected bool IsActivated
    {
        get
        {
            return StateIsPlaying && CanBeActivated();
        }
    }

    public event ActivationHandler CanBeActivatedChanged;

    protected void InvokeCanBeActivatedChanged()
    {
        Assert.IsFalse(CanBeActivatedChanged == null, this + " The correpsonding ability should always listen");
        CanBeActivatedChanged.Invoke(IsActivated);
    }

    protected virtual bool CanBeActivated()
    {
        return true;
    }


    public virtual bool HasIcon()
    {
        return true;
    }

    public bool Activate()
    {
        return FirstActivation();
    }

    public bool Reactivate()
    {
        return SecondActivation();
    }

    public bool Cancel()
    {
        return Cancellation();
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

    public virtual string GetKey()
    {
        return "";
    }

    protected void Awake()
    {
        Players.MyPlayer.StateChanged += PlayerStateChanged;
    }

    private void PlayerStateChanged(Player.State newState)
    {
        bool previousActivation = IsActivated;
        StateIsPlaying = newState == Player.State.PLAYING || (newState == Player.State.NO_MOVEMENT && !IsMovement());
        if (previousActivation != IsActivated)
            InvokeCanBeActivatedChanged();
    }

    protected virtual void Start()
    {
        if (HasIcon())
        {
            GameObject keyToUsePrefab = Resources.Load<GameObject>(Paths.KEY_TO_USE);
            GameObject keyToUse = Instantiate(keyToUsePrefab);
            keyToUse.transform.SetParent(transform, false);
            keyToUse.GetComponentInChildren<Text>().text = GetKey();
        }
        PlayerStateChanged(Players.MyPlayer.CurrentState);
    }

    protected virtual void OnDestroy()
    {
        if (Players.MyPlayer != null)
            Players.MyPlayer.StateChanged -= PlayerStateChanged;
    }

    protected virtual bool IsMovement()
    {
        return false;
    }

}