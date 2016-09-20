using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AbilityEffect))]
[RequireComponent(typeof(AbilityTargeting))]
[RequireComponent(typeof(AbilityInput))]
public class Ability : MonoBehaviour
{
    private AbilityInput input;
    private AbilityTargeting targeting;
    private AbilityEffect effect;
    private Image UI;

    public bool NoCooldown = false;
    public float cooldownDuration = 5;
    private float currentCooldown = 0;

    private enum State
    {
        READY,
        CHOOSINGTARGET,
        LOADING,
    }

    private State state = State.READY;

    // Use this for initialization
    void Start()
    {
        UI = GetComponent<Image>();
        UI.fillAmount = 0;
        input = GetComponent<AbilityInput>();
        targeting = GetComponent<AbilityTargeting>();
        effect = GetComponent<AbilityEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.READY:
                if (input.Activated())
                {
                    CastAbility();
                }
                break;
            case State.CHOOSINGTARGET:
                break;
            case State.LOADING:
                currentCooldown = Mathf.Max(0f, currentCooldown - Time.deltaTime);
                UI.fillAmount = currentCooldown / cooldownDuration;
                if (currentCooldown == 0)
                {
                    state = State.READY;
                }
                break;
        }
    }

    private void CastAbility()
    {
        if (currentCooldown == 0)
        {
            state = State.CHOOSINGTARGET;
            targeting.ChooseTarget(CastOnTarget);
        }
    }

    private void CastOnTarget(GameObject target, Vector3 position)
    {
        effect.ApplyOnTarget(target, position);
        currentCooldown = cooldownDuration;
        if (NoCooldown)
            state = State.READY;
        else
            state = State.LOADING;
    }
}
