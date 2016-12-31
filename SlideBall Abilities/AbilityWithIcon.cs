using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityWithIcon : Ability
{
    private Image CooldownOverlay;
    private GameObject DisabledOverlay;

    protected override void Awake()
    {
        base.Awake();
        CreateToolTip();

        GameObject CooldownOverlayPrefab = Resources.Load<GameObject>(Paths.ABILITY_COOLDOWN);
        CooldownOverlay = ((GameObject)Instantiate(CooldownOverlayPrefab, transform, false)).GetComponent<Image>();
        //
        GameObject DisabledOverlayPrefab = Resources.Load<GameObject>(Paths.ABILITY_DISABLED);
        DisabledOverlay = (GameObject)Instantiate(DisabledOverlayPrefab, transform, false);
    }

    private void CreateToolTip()
    {
        AbilityTooltip toolTip = gameObject.AddComponent<AbilityTooltip>();

        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();
        //
        EventTrigger.Entry enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerEnter;
        enter.callback.AddListener((eventData) => toolTip.MouseEnter());
        trigger.triggers.Add(enter);
        //
        EventTrigger.Entry exit = new EventTrigger.Entry();
        exit.eventID = EventTriggerType.PointerExit;
        exit.callback.AddListener((eventData) => toolTip.MouseExit());
        trigger.triggers.Add(exit);
    }

    void Start()
    {
        CooldownOverlay.fillAmount = 0;
    }

    protected override void UpdateUI()
    {
        CooldownOverlay.fillAmount = currentCooldown / cooldownDuration;
    }

    protected override void EnableAbility(bool enable)
    {
        base.EnableAbility(enable);
        if (enable)
            DisabledOverlay.SetActive(false);
        else
            DisabledOverlay.SetActive(true);
    }
}
