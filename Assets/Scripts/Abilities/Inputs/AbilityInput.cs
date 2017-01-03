

using PlayerManagement;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public delegate void ActivationHandler(bool activated);

//Manage when an ability can be activated (The game is p
public abstract class AbilityInput : MonoBehaviour
{
    protected bool CurrentStateAllowActivation;

    protected bool IsActivated
    {
        get
        {
            return CurrentStateAllowActivation && AdditionalActivationRequirementsAreMet();
        }
    }

    public event ActivationHandler CanBeActivatedChanged;

    protected void InvokeCanBeActivatedChanged()
    {
        Assert.IsFalse(CanBeActivatedChanged == null, this + " The correpsonding ability should always listen");
        CanBeActivatedChanged.Invoke(IsActivated);
    }

    protected virtual bool AdditionalActivationRequirementsAreMet()
    {
        return true;
    }


    public virtual bool HasIcon()
    {
        return true;
    }

    public abstract bool FirstActivation();

    //Sometimes, 
    public virtual bool SecondActivation()
    {
        return false;
    }

    public virtual bool Cancellation()
    {
        return false;
    }

    //The letter that should be shown if the ability has an icon
    public virtual string GetKey()
    {
        return "";
    }

    protected void Awake()
    {
        Players.MyPlayer.StateChanged += PlayerStateChanged;
    }

    //Check if the ability can be activated depending on the new state of the player.
    private void PlayerStateChanged(Player.State newState)
    {
        bool previousActivation = IsActivated;
        CurrentStateAllowActivation = newState == Player.State.PLAYING || (newState == Player.State.NO_MOVEMENT && !IsMovement());
        if (previousActivation != IsActivated)
            InvokeCanBeActivatedChanged();
    }

    protected virtual void Start()
    {
        //If the ability has an icon, add a small GUI rectangle with the letter corresponding to the key.
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