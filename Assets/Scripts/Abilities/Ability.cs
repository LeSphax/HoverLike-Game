using UnityEngine;

[RequireComponent(typeof(AbilityEffect))]
[RequireComponent(typeof(AbilityTargeting))]
[RequireComponent(typeof(AbilityInput))]
//Manages the state of the ability and its cooldown. Links the input, targeting and effects components of the ability.
public class Ability : MonoBehaviour
{
    private AbilityInput input;
    private AbilityTargeting targeting;
    private AbilityEffect[] effects;

    public bool NoCooldown = false;
    public float cooldownDuration = 5;
    protected float currentCooldown = 0;

    public bool trace;

    private enum State
    {
        READY,
        CHOOSINGTARGET,
        LOADING,
    }

    private State state = State.READY;

    protected bool isEnabled;

    protected virtual void Awake()
    {
        input = GetComponent<AbilityInput>();
        input.CanBeActivatedChanged += EnableAbility;
        targeting = GetComponent<AbilityTargeting>();
        effects = GetComponents<AbilityEffect>();
        if (EditorVariables.NoCooldowns)
            cooldownDuration = Mathf.Min(cooldownDuration, 1);
    }


    protected virtual void OnDestroy()
    {
        input.CanBeActivatedChanged -= EnableAbility;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (trace)
        {
            Debug.LogWarning(state);
        }
#endif
        switch (state)
        {
            case State.READY:
                if (input.FirstActivation() && isEnabled)
                {
                    CastAbility();
                }
                //Used to stop the visual effect for the brake effect
                input.Cancellation();
                break;
            case State.CHOOSINGTARGET:
                if (input.Cancellation())
                {
                    state = State.READY;
                    targeting.CancelTargeting();
                }
                if (input.SecondActivation())
                {
                    state = State.CHOOSINGTARGET;
                    targeting.ReactivateTargeting();
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

    private void CastOnTarget(bool wasUsed, params object[] parameters)
    {
        if (wasUsed)
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
        else
        {
            state = State.READY;
        }
    }

    protected virtual void EnableAbility(bool enable)
    {
        isEnabled = enable;
        if (!isEnabled)
            if (state == State.CHOOSINGTARGET)
            {
                targeting.CancelTargeting();
                state = State.READY;
            }
    }
}
