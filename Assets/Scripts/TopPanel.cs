using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TopPanel : MonoBehaviour {

    public Text m_Status;
    public Text m_RoomName;

    public event EmptyEventHandler BackPressed;

    public UserSettingsPanel settingsPanel;

    private void Awake()
    {
        if (UserSettings.Nickname == "")
            settingsPanel.Open();
        else
            settingsPanel.Close();
    }

    public string Status
    {
        get
        {
            return m_Status.text;
        }
        set
        {
            m_Status.text = value;
        }
    }

    public string RoomName
    {
        get
        {
            return m_RoomName.text;
        }
        set
        {
            m_RoomName.text = value;
        }
    }

    public void BackButton()
    {
        BackPressed.Invoke();
    }

    public void SettingsButton()
    {
        settingsPanel.Open();
    }
}
