using UnityEngine;

public class MenuInGame : MonoBehaviour {

    public void OpenSettings()
    {
        UserSettingsPanel.InstantiateSettingsPanel().transform.SetParent(transform.parent, false);
    }
}
