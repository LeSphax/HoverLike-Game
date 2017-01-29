using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class AbilityTooltip : MonoBehaviour
{
    Tooltip tooltip;

    // Use this for initialization
    void Start()
    {
        Type abilityType = GetComponentInChildren<AbilityTargeting>().GetType();
        string abilityName = abilityType.FullName.Remove(abilityType.FullName.Length - "Targeting".Length);


        GameObject tooltipPrefab = Resources.Load<GameObject>(Paths.TOOLTIP);
        tooltip = ((GameObject)Instantiate(tooltipPrefab, transform.parent.parent, false)).GetComponent<Tooltip>();
        tooltip.gameObject.name = abilityName;
        tooltip.gameObject.SetActive(false);

        tooltip.Content = Language.Instance.texts[abilityName];
        tooltip.Title = Language.Instance.texts[abilityName + "Title"];
        tooltip.Icon = GetComponent<Image>().sprite;

    }

    public void MouseEnter()
    {
        Invoke("ActivateTooltip", 0.5f);
    }

    private void Update()
    {
        if (tooltip.gameObject.activeInHierarchy)
            PutTooltipOnMouse();
    }

    public void MouseExit()
    {
        tooltip.gameObject.SetActive(false);
        CancelInvoke("ActivateTooltip");
    }

    public void ActivateTooltip()
    {
        tooltip.gameObject.SetActive(true);
        PutTooltipOnMouse();
    }

    void PutTooltipOnMouse()
    {
        tooltip.transform.position = Input.mousePosition;
    }

    private void OnDestroy()
    {
        if (tooltip != null)
            Destroy(tooltip.gameObject);
    }

}
