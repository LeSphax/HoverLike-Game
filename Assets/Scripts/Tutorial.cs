using PlayerManagement;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public GameObject tutorialPanel;
    public GameObject LeftClickGif;
    public GameObject RightClickGif;
    public GameObject AbilitiesImage;
    public Text Explanation;
    public Text buttonContent;

    public enum State
    {
        RIGHT,
        LEFT,
        ABILITIES,
        CLOSED,
    }

    private State myState;
    private State MyState
    {
        set
        {
            myState = value;
            switch (value)
            {
                case State.RIGHT:
                    tutorialPanel.SetActive(true);
                    LeftClickGif.SetActive(false);
                    RightClickGif.SetActive(true);
                    AbilitiesImage.SetActive(false);
                    Explanation.text = Language.Instance.texts["Tuto_RC"];
                    buttonContent.text = Language.Instance.texts["Button_Next"];
                    break;
                case State.LEFT:
                    tutorialPanel.SetActive(true);
                    LeftClickGif.SetActive(true);
                    RightClickGif.SetActive(false);
                    AbilitiesImage.SetActive(false);
                    Explanation.text = Language.Instance.texts["Tuto_LC"];
                    buttonContent.text = Language.Instance.texts["Button_Next"];
                    break;
                case State.ABILITIES:
                    tutorialPanel.SetActive(true);
                    LeftClickGif.SetActive(false);
                    RightClickGif.SetActive(false);
                    AbilitiesImage.SetActive(true);
                    Explanation.text = Language.Instance.texts["Tuto_Abilities"];
                    buttonContent.text = Language.Instance.texts["Button_Close"];
                    break;
                case State.CLOSED:
                    tutorialPanel.SetActive(false);
                    break;
                default:
                    break;
            }
        }
    }

    public void Start()
    {
        if (!UserSettings.SeenTutorial)
        {
            Reset();
            UserSettings.SeenTutorial = true;
        }
        else
        {
            MyState = State.CLOSED;
        }
    }

    public void Reset()
    {
        MyState = State.RIGHT;
    }

    public void SetState(State state)
    {
        MyState = state;
    }

    public void ButtonPressed()
    {
        switch (myState)
        {
            case State.RIGHT:
                MyState = State.LEFT;
                break;
            case State.LEFT:
                MyState = State.ABILITIES;
                break;
            case State.ABILITIES:
                MyState = State.CLOSED;
                break;
            case State.CLOSED:
                Debug.LogError("This shouldn't happen");
                break;
            default:
                break;
        }
    }
}
