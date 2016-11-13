using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbilityEffectBuilder))]
[RequireComponent(typeof(AbilityTargeting))]
[RequireComponent(typeof(AbilityInput))]
public class Ability : MonoBehaviour
{
    private AbilityInput input;
    private AbilityTargeting targeting;
    private AbilityEffectBuilder[] effectBuilders;

    public bool NoCooldown = false;
    public float cooldownDuration = 5;

    private bool activate;
    private bool reactivate;
    private bool cancel;

    void Update()
    {
        if (input.Activate())
        {
            activate = true;
        }
        if (input.Reactivate())
        {
            reactivate = true;
        }
        if (input.Cancel())
            cancel = true;
    }

    public void ResetInputs()
    {
        activate = false;
        reactivate = false;
        cancel = false;
    }

    internal List<AbilityEffect> UpdateAbility()
    {
        switch (state)
        {
            case State.READY:
                if (Enabled)
                {
                    if (activate)
                    {
                        if (currentCooldown == 0)
                        {
                            state = State.CHOOSINGTARGET;
                            return targeting.StartTargeting(CastOnTarget);
                        }
                    }
                }
                return null;
            case State.CHOOSINGTARGET:
                if (Enabled)
                {
                    if (cancel)
                    {
                        targeting.CancelTargeting();
                        state = State.READY;
                    }
                    else if (reactivate)
                    {
                        return targeting.ReactivateTargeting();
                    }
                }
                return null;
            case State.LOADING:
                currentCooldown = Mathf.Max(0f, currentCooldown - Time.deltaTime);
                UpdateUI();
                if (currentCooldown == 0)
                {
                    state = State.READY;
                }
                return null;
            default:
                throw new UnhandledSwitchCaseException(state);
        }
    }

    protected float currentCooldown = 0;

    private enum State
    {
        READY,
        CHOOSINGTARGET,
        LOADING,
    }

    private State state = State.READY;

    protected bool isEnabled;
    protected virtual bool Enabled
    {
        get
        {
            return isEnabled;
        }
        set
        {
            isEnabled = value;
            if (!isEnabled)
                if (state == State.CHOOSINGTARGET)
                {
                    targeting.CancelTargeting();
                    state = State.READY;
                }
        }
    }

    protected virtual void Awake()
    {
        input = GetComponent<AbilityInput>();
        input.CanBeActivatedChanged += EnableAbility;
        targeting = GetComponent<AbilityTargeting>();
        effectBuilders = GetComponents<AbilityEffectBuilder>();
        MyComponents.AbilitiesManager.RegisterAbility(this);
    }


    protected void OnDestroy()
    {
        MyComponents.AbilitiesManager.UnregisterAbility(this);
        input.CanBeActivatedChanged -= EnableAbility;
    }

    protected virtual void UpdateUI() { }

    private List<AbilityEffect> CastOnTarget(params object[] parameters)
    {
        List<AbilityEffect> effects = new List<AbilityEffect>();
        foreach (AbilityEffectBuilder effectBuilder in effectBuilders)
            effects.Add(effectBuilder.GetEffect(parameters));
        if (NoCooldown)
            state = State.READY;
        else
        {
            currentCooldown = cooldownDuration;
            state = State.LOADING;
        }
        return effects;
    }

    private void EnableAbility(bool enable)
    {
        Enabled = enable;
    }
}
