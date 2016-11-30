using UnityEngine;
using System.Collections;
using System;

public class AbilityTooltip : MonoBehaviour
{
    Tooltip tooltip;

    // Use this for initialization
    void Start()
    {
        Type abilityType = GetComponentInChildren<AbilityTargeting>().GetType();
        string key = abilityType.FullName.Remove(abilityType.FullName.Length - "Targeting".Length);


        GameObject tooltipPrefab = Resources.Load<GameObject>(Paths.TOOLTIP);
        tooltip = ((GameObject)Instantiate(tooltipPrefab, transform.parent.parent, false)).GetComponent<Tooltip>();
        tooltip.gameObject.name = key;
        tooltip.gameObject.SetActive(false);

        tooltip.Content = Language.Instance.texts[key];
        tooltip.Title = Language.Instance.texts[key + "Title"];


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

}
