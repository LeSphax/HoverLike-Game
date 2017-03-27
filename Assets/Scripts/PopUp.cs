using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    public GameObject popUp;
    public Text content;
    public Text buttonText;
    public Image icon;

    enum State
    {
        CLOSED,
        OPEN,
    }

    private State __state;
    private State state
    {
        get
        {
            return __state;
        }
        set
        {
            __state = value;
            SetPopUpActivation(value);
        }
    }

    private void Awake()
    {
        SceneManager.sceneLoaded += ChangedScene;
    }

    public void Show(string text, Sprite image = null) {
        Show(text, Language.Instance.texts["Button_Ok"], image);
    }

    public void Show(string text, string buttonText, Sprite image = null)
    {
        state = State.OPEN;
        content.text = text;
        this.buttonText.text = buttonText;
        this.icon.sprite = image;
    }

    public void ClosePopUp()
    {
        state = State.CLOSED;
    }

    private void ChangedScene(Scene scene, LoadSceneMode mode)
    {
        SetPopUpActivation(state);
    }

    private void SetPopUpActivation(State state)
    {
        switch (state)
        {
            case State.CLOSED:
                popUp.SetActive(false);
                break;
            case State.OPEN:
                popUp.SetActive(true);
                break;
            default:
                break;
        }
    }
}
