using UnityEngine;

[RequireComponent(typeof(AbilityEffect))]
[RequireComponent(typeof(AbilityTargeting))]
[RequireComponent(typeof(AbilityInput))]
//Manages the state of the ability and its cooldown. Links the input, targeting and effects components of the ability.
public class Ability : MonoBehaviour
{

    private static event EmptyEventHandler NewAbilityUsed;
    private AbilityInput input;
    private AbilityTargeting targeting;
    private AbilityEffect[] effects;

    public bool NoCooldown = false;
    public float cooldownDuration = 5;
    protected float currentCooldown = 0;

    public bool trace;

    private static AudioClip errorSound;

    private enum State
    {
        READY,
        TRYING_TO_ACTIVATE,
        CHOOSING_TARGET,
        APPLYING_EFFECT,
        LOADING,
    }

    private State state = State.READY;

    protected bool isEnabled;

    protected virtual void Awake()
    {
        if (errorSound == null)
        {
            errorSound = ResourcesGetter.SoftErrorSound;
        }
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
            Debug.LogWarning(state + "   " + isEnabled + "    " + currentCooldown);
        }
#endif
        if (SlideBallInputs.GetKeyDown(UserSettings.GetKeyCode(5), SlideBallInputs.GUIPart.ABILITY))
        {
            TryCancelTargeting();
        }
        switch (state)
        {
            case State.READY:
                if (input.FirstActivation())
                {
                    if (isEnabled)
                        CastAbility();
                    else
                        state = State.TRYING_TO_ACTIVATE;
                }
                break;
            case State.TRYING_TO_ACTIVATE:
                if (input.FirstActivation() || input.ContinuousActivation())
                {
                    if (isEnabled)
                        CastAbility();
                }
                break;
            case State.CHOOSING_TARGET:
                if (input.Cancellation())
                {
                    CancelTargeting();
                }
                if (input.SecondActivation())
                {
                    state = State.CHOOSING_TARGET;
                    targeting.ReactivateTargeting();
                }
                break;
            case State.APPLYING_EFFECT:
                if (input.Cancellation())
                {
                    CastCancelAbility();
                }
                break;
            case State.LOADING:
                currentCooldown = Mathf.Max(0f, currentCooldown - Time.deltaTime);
                UpdateUI();
                if (currentCooldown == 0)
                {
                    state = State.READY;
                    Update();
                }
                else if (input.FirstActivation())
                {
                    PlayErrorSound();
                }
                break;
            default:
                throw new UnhandledSwitchCaseException(state);
        }
    }

    private void CancelTargeting()
    {
        state = State.READY;
        targeting.CancelTargeting();
    }

    private void PlayErrorSound()
    {
        if (input.HasErrorSound())
            MyComponents.GlobalSound.Play(errorSound);
    }

    protected virtual void UpdateUI() { }

    private void CastAbility()
    {
        if (NewAbilityUsed != null)
        {
            NewAbilityUsed.Invoke();
        }
        if (currentCooldown == 0)
        {
            state = State.CHOOSING_TARGET;
            targeting.ChooseTarget(CastOnTarget);
        }
    }

    private void CastCancelAbility()
    {
        targeting.ChooseTarget(CastOnCancelTarget);
    }

    private void CastOnCancelTarget(bool wasUsed, params object[] parameters)
    {
        if (wasUsed)
        {
            foreach (AbilityEffect effect in effects)
            {

                if (effect.LongEffect)
                    effect.ApplyOnTargetCancel(parameters);
            }
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

    private void CastOnTarget(bool wasUsed, params object[] parameters)
    {
        if (wasUsed)
        {
            if (trace)
                Debug.Log("Cast On Target");
            bool hasLongEffect = false;
            foreach (AbilityEffect effect in effects)
            {
                effect.ApplyOnTarget(parameters);
                if (effect.LongEffect)
                    hasLongEffect = true;
            }
            if (trace)
                Debug.Log(gameObject.name + " Long Effect " + hasLongEffect);
            if (hasLongEffect)
            {
                state = State.APPLYING_EFFECT;
            }
            else if (NoCooldown)
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
            if (state == State.CHOOSING_TARGET)
            {
                targeting.CancelTargeting();
                state = State.READY;
            }
    }

    private void OnEnable()
    {
        NewAbilityUsed += TryCancelTargeting;
    }

    private void OnDisable()
    {
        NewAbilityUsed -= TryCancelTargeting;
    }

    private void TryCancelTargeting()
    {
        switch (state)
        {
            case State.READY:
                break;
            case State.TRYING_TO_ACTIVATE:
            case State.CHOOSING_TARGET:
                //CancelTargeting();
                break;
            case State.LOADING:
                break;
            default:
                break;
        }
    }
}
