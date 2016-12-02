using UnityEngine;
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
        set
        {
            switch (value)
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

}
