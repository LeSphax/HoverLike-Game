using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UserSettingsPanel : MonoBehaviour
{

    public Button[] buttons;
    public InputField nicknameField;
    public Text nicknamePlaceholder;
    public Slider volumeSlider;

    private SlideBallInputs.GUIPart previousGUIPart;

    private int? currentButton;

    public static GameObject InstantiateSettingsPanel()
    {
        GameObject panel = Instantiate(ResourcesGetter.SettingsPanelPrefab);
        return panel;
    }


    // Use this for initialization
    void Start()
    {
        for(int i= 0; i<buttons.Length; i++)
        {
            buttons[i].onClick.AddListener(() => { ButtonPressed(i); });
        }
        currentButton = null;
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentButton != null)
        {
            foreach (char c in Input.inputString)
            {
                if ((c >= ' ' && c <= 'z'))
                {
                    SetChar(currentButton.Value, c);
                    currentButton = null;
                }
            }
        }
    }

    private void Reset()
    {
        previousGUIPart = SlideBallInputs.currentPart;
        SlideBallInputs.currentPart = SlideBallInputs.GUIPart.MENU;
        nicknamePlaceholder.text = Random_Name_Generator.GetRandomName();
        nicknameField.text = UserSettings.Nickname;
        volumeSlider.value = UserSettings.Volume;
        for (int i = 0; i < buttons.Length; i++)
        {
            char key = UserSettings.GetKey(i);
            SetChar(i, key);
        }

        nicknameField.Select();
        nicknameField.ActivateInputField();
    }

    public void ButtonPressed(int number)
    {
        currentButton = number;
        SetChar(number, (char?)null);
    }

    private void SetChar(int number, char? c)
    {
        string text;
        if (c == ' ')
        {
            text = "Space";
        }
        else if (c == null)
        {
            text = " ";
        }
        else
        {
            c = char.ToUpper(c.Value);
            text = c.ToString();
        }
        SetText(number, text);
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i != number && GetText(i) == text)
            {
                SetText(i, "n/a");
            }
        }
    }

    private void SetText(int number, string text)
    {
        buttons[number].GetComponentInChildren<Text>().text = text;
    }

    private string GetText(int number)
    {
        return buttons[number].GetComponentInChildren<Text>().text;
    }

    public void Save()
    {
        if (ValidConfiguration())
        {
            if (nicknameField.text == "")
            {
                UserSettings.Nickname = nicknamePlaceholder.text;
            }
            else
            {
                UserSettings.Nickname = nicknameField.text;
            }
            string keys = "";
            for (int i = 0; i < buttons.Length; i++)
            {
                string text = GetText(i);
                text = text == "Space" ? " " : text;
                keys += text;
            }
            UserSettings.SetKeys(keys);
            UserSettings.Volume = volumeSlider.value;
            Close();
        }
        else
        {
            MyComponents.PopUp.Show(Language.Instance.texts["Settings_Problem"]);
        }
    }

    private bool ValidConfiguration()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            string text = GetText(i);
            if (text == "n/a" || text == " ")
            {
                return false;
            }
        }
        return true;
    }

    public void Cancel()
    {
        Close();
    }

    public void Close()
    {
        SlideBallInputs.currentPart = previousGUIPart;
        Destroy(gameObject);
    }
}
