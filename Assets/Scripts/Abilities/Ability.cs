using UnityEngine;

[RequireComponent(typeof(AbilityEffect))]
[RequireComponent(typeof(AbilityTargeting))]
[RequireComponent(typeof(AbilityInput))]
public class Ability : MonoBehaviour
{
    private AbilityInput input;
    private AbilityTargeting targeting;
    private AbilityEffect[] effects;

    public bool NoCooldown = false;
    public float cooldownDuration = 5;
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
        effects = GetComponents<AbilityEffect>();
    }


    protected void OnDestroy()
    {
        input.CanBeActivatedChanged -= EnableAbility;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.READY:
                if (input.Activate() && Enabled)
                {
                    CastAbility();
                }
                break;
            case State.CHOOSINGTARGET:
                if (input.Cancel())
                {
                    targeting.CancelTargeting();
                    state = State.READY;
                }
                break;
            case State.LOADING:
                currentCooldown = Mathf.Max(0f, currentCooldown - Time.deltaTime);
                UpdateUI();
                if (currentCooldown == 0)
                {
                    state = State.READY;
                }
                break;
            default:
                throw new UnhandledSwitchCaseException(state);
        }
    }

    protected virtual void UpdateUI() { }

    private void CastAbility()
    {
        if (currentCooldown == 0)
        {
            state = State.CHOOSINGTARGET;
            targeting.ChooseTarget(CastOnTarget);
        }
    }

    private void CastOnTarget(params object[] parameters)
    {
        foreach (AbilityEffect effect in effects)
            effect.ApplyOnTarget(parameters);
        if (NoCooldown)
            state = State.READY;
        else
        {
            currentCooldown = cooldownDuration;
            state = State.LOADING;
        }
    }

    private void EnableAbility(bool enable)
    {
        Enabled = enable;
    }
}
