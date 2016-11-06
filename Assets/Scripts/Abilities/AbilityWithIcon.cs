using UnityEngine;
using UnityEngine.UI;

public class AbilityWithIcon : Ability
{
    private Image CooldownOverlay;
    private GameObject DisabledOverlay;

    protected override bool Enabled
    {
        get
        {
            return base.Enabled;
        }

        set
        {
            base.Enabled = value;
            if (value)
                DisabledOverlay.SetActive(false);
            else
                DisabledOverlay.SetActive(true);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        CooldownOverlay = GetComponent<Image>();
        GameObject DisabledOverlayPrefab = Resources.Load<GameObject>("Abilities/Disabled");
        DisabledOverlay = (GameObject)Instantiate(DisabledOverlayPrefab, transform, false);
    }

    void Start()
    {
        CooldownOverlay.fillAmount = 0;
    }

    protected override void UpdateUI()
    {
        CooldownOverlay.fillAmount = currentCooldown / cooldownDuration;
    }
}
