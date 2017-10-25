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

        CooldownOverlay = Instantiate(ResourcesGetter.CooldownPrefab, transform, false).GetComponent<Image>();
        //
        DisabledOverlay = Instantiate(ResourcesGetter.DisabledPrefab, transform, false);
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
        CooldownOverlay.fillAmount = currentCooldown / CooldownDuration;
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
