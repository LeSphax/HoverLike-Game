using UnityEngine;
using UnityEngine.UI;

public class ChatMessageWriter : MonoBehaviour
{
    public bool hideWhenNotActive;
    public InputField input;
    public ChatTargetOfMessageLabel target;

    public event EmptyEventHandler ChangeContext;

    private void Awake()
    {
        if (hideWhenNotActive)
            ActivateInput(false);
    }

    private void Update()
    {
        if (SlideBallInputs.currentPart != SlideBallInputs.GUIPart.MENU)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
                ChangeContext.Invoke();
            if (SlideBallInputs.AnyEnterDown())
            {
                if (input.gameObject.activeSelf)
                    SendContent(input.text, target.SendToAll);
                if (hideWhenNotActive)
                {
                    ActivateInput(!input.gameObject.activeSelf);
                }
                else
                {
                    FocusOnInputField();
                }
            }
        }
    }

    private void FocusOnInputField()
    {
        input.Select();
        input.ActivateInputField();
    }

    private void ActivateInput(bool active)
    {
        input.gameObject.SetActive(active);
        if (input.gameObject.activeSelf)
        {
            SlideBallInputs.currentPart = SlideBallInputs.GUIPart.CHAT;
            FocusOnInputField();
        }
        else
            SlideBallInputs.currentPart = Scenes.CurrentSceneDefaultGUIPart();
    }

    public void SendContent(string content, bool sendToAll)
    {
        if (content != "")
        {
            MyComponents.ChatManager.UserWriteMessage(content, sendToAll);
            input.text = "";
        }
    }
}
