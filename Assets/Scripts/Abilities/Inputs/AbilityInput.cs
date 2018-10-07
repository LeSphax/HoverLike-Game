﻿using PlayerManagement;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public delegate void ActivationHandler(bool activated);

//Manage when an ability can be activated (We are actually during a round, the player has the ball if it is necessary for the ability)
public abstract class AbilityInput : SlideBall.MonoBehaviour
{
    protected virtual int INPUT_NUMBER
    {
        get
        {
            return -1;
        }
    }

    protected bool previousActivation;

    protected bool IsActivated
    {
        get
        {
            return CurrentStateAllowActivation(MyComponents.Players.MyPlayer) && AdditionalActivationRequirementsAreMet();
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

    public virtual bool FirstActivation()
    {
        return MyComponents.InputManager.GetKeyDown(UserSettings.KeyForInputCheck(INPUT_NUMBER), GUIPart.ABILITY);
    }

    public virtual bool ContinuousActivation()
    {
        if (INPUT_NUMBER != -1)
            return MyComponents.InputManager.GetKey(UserSettings.KeyForInputCheck(INPUT_NUMBER), GUIPart.ABILITY);
        return false;
    }

    //For abilities that need targeting, there is a first phase with the targeting UI and a second one to actually fire the ability.
    public virtual bool SecondActivation()
    {
        return false;
    }

    public virtual bool Cancellation()
    {
        return false;
    }

    //The letter that should be shown if the ability has an icon
    public virtual string GetKeyForGUI()
    {
        return "";
    }

    //Check if the ability can be activated depending on the new state of the player.
    protected void PlayerStateChanged(Player player)
    {
        if (previousActivation != IsActivated)
        {
            previousActivation = IsActivated;
            InvokeCanBeActivatedChanged();
        }
    }

    protected virtual bool CurrentStateAllowActivation(Player player)
    {
        return player.State.Movement == MovementState.PLAYING || (player.State.Movement == MovementState.NO_MOVEMENT && !IsMovement());
    }

    protected virtual void Start()
    {
        MyComponents.Players.MyPlayer.eventNotifier.ListenToEvents(PlayerStateChanged, PlayerFlags.MOVEMENT_STATE);
        //If the ability has an icon, add a small GUI rectangle with the letter corresponding to the key.
        if (HasIcon())
        {
            GameObject keyToUse = Instantiate(ResourcesGetter.KeyToUsePrefab);
            keyToUse.transform.SetParent(transform, false);
            keyToUse.GetComponentInChildren<Text>().text = GetKeyForGUI();
        }
        PlayerStateChanged(MyComponents.Players.MyPlayer);
    }

    protected virtual void OnDestroy()
    {
        if (MyComponents.Players.MyPlayer != null)
            MyComponents.Players.MyPlayer.eventNotifier.StopListeningToEvents(PlayerStateChanged, PlayerFlags.MOVEMENT_STATE);
    }

    protected virtual bool IsMovement()
    {
        return false;
    }

    public virtual bool HasErrorSound()
    {
        return true;
    }

}