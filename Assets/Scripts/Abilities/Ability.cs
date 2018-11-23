using Byn.Net;
using UnityEngine;

[RequireComponent(typeof(AbilityEffect))]
[RequireComponent(typeof(AbilityTargeting))]
[RequireComponent(typeof(AbilityInput))]
//Manages the state of the ability and its cooldown. Links the input, targeting and effects components of the ability.
public class Ability : SlideBall.MonoBehaviour
{

    private AbilityInput Input;
    private AbilityTargeting Targeting;
    private AbilityEffect[] Effects;
    [HideInInspector]
    private ConnectionId playerId = ConnectionId.INVALID;
    public ConnectionId PlayerId
    {
        get
        {
            return playerId;
        }
        set
        {
            playerId = value;
            Input.PlayerId = value;
            Targeting.PlayerId = value;
            foreach (AbilityEffect effect in Effects)
            {
                effect.PlayerId = value;
            }
        }
    }

    public bool NoCooldown = false;
    public float CooldownDuration = 5;
    protected float currentCooldown = 0;

    public bool Trace;

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
        Input = GetComponent<AbilityInput>();
        Input.CanBeActivatedChanged += EnableAbility;
        Targeting = GetComponent<AbilityTargeting>();
        Effects = GetComponents<AbilityEffect>();
        if (EditorVariables.NoCooldowns)
            CooldownDuration = Mathf.Min(CooldownDuration, 1);
    }


    protected virtual void OnDestroy()
    {
        Input.CanBeActivatedChanged -= EnableAbility;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Trace)
        {
            Debug.LogWarning(state + "   " + isEnabled + "    " + currentCooldown);
        }
#endif
        if (MyComponents.Players.players[PlayerId].InputManager.GetKeyDown(UserSettings.KeyForInputCheck(5), GUIPart.ABILITY))
        {
            TryCancelTargeting();
        }
        switch (state)
        {
            case State.READY:
                if (Input.FirstActivation())
                {
                    if (isEnabled)
                        CastAbility();
                    else
                        state = State.TRYING_TO_ACTIVATE;
                }
                break;
            case State.TRYING_TO_ACTIVATE:
                if (Input.FirstActivation() || Input.ContinuousActivation())
                {
                    if (isEnabled)
                        CastAbility();
                }
                break;
            case State.CHOOSING_TARGET:
                if (Input.Cancellation())
                {
                    CancelTargeting();
                }
                if (Input.SecondActivation())
                {
                    state = State.CHOOSING_TARGET;
                    Targeting.ReactivateTargeting();
                }
                break;
            case State.APPLYING_EFFECT:
                if (Input.Cancellation())
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
                else if (Input.FirstActivation())
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
        Targeting.CancelTargeting();
    }

    private void PlayErrorSound()
    {
        if (Input.HasErrorSound())
            MyComponents.GlobalSound.Play(errorSound);
    }

    protected virtual void UpdateUI() { }

    private void CastAbility()
    {
        if (currentCooldown == 0)
        {
            state = State.CHOOSING_TARGET;
            Targeting.ChooseTarget(CastOnTarget);
        }
    }

    private void CastCancelAbility()
    {
        Targeting.ChooseTarget(CastOnCancelTarget);
    }

    private void CastOnCancelTarget(bool wasUsed, params object[] parameters)
    {
        if (wasUsed)
        {
            foreach (AbilityEffect effect in Effects)
            {

                if (effect.LongEffect)
                    effect.ApplyOnTargetCancel(parameters);
            }
            if (NoCooldown)
                state = State.READY;
            else
            {
                SetOnCooldown();
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
            if (Trace)
                Debug.Log("Cast On Target");
            bool hasLongEffect = false;
            foreach (AbilityEffect effect in Effects)
            {
                effect.ApplyOnTarget(parameters);
                if (effect.LongEffect)
                    hasLongEffect = true;
            }
            if (Trace)
                Debug.Log(gameObject.name + " Long Effect " + hasLongEffect);
            if (hasLongEffect)
            {
                state = State.APPLYING_EFFECT;
            }
            else if (NoCooldown)
                state = State.READY;
            else
            {
                SetOnCooldown();
            }
        }
        else
        {
            state = State.READY;
        }
    }

    public void SetOnCooldown()
    {
        currentCooldown = CooldownDuration * (1 - EditorVariables.CooldownReduction);
        state = State.LOADING;
    }

    protected virtual void EnableAbility(bool enable)
    {
        isEnabled = enable;
        if (!isEnabled)
            if (state == State.CHOOSING_TARGET)
            {
                Targeting.CancelTargeting();
                state = State.READY;
            }
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
