using UnityEngine;

public class MenuInGame : MonoBehaviour {

    SlideBallInputs.GUIPart previousPart;

    public void OpenSettings()
    {
        UserSettingsPanel.InstantiateSettingsPanel().transform.SetParent(transform.parent, false);
    }

    public void OpenMenu()
    {
        gameObject.SetActive(true);
        previousPart = SlideBallInputs.currentPart;
        SlideBallInputs.currentPart = SlideBallInputs.GUIPart.MENU;
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
        SlideBallInputs.currentPart = previousPart;
    }
}
