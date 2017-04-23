using UnityEngine;
using UnityEngine.UI;

public class ChatMessageWriter : MonoBehaviour
{
    public bool hideWhenNotActive;
    public InputField input;

    public event EmptyEventHandler SendToTeamActivated;
    public event EmptyEventHandler SendToAllActivated;

    private void Awake()
    {
        if (hideWhenNotActive)
            ActivateInput(false);
    }

    private void Update()
    {
        if (SlideBallInputs.currentPart != SlideBallInputs.GUIPart.MENU)
        {
            if (SlideBallInputs.AnyShiftDown())
                SendToAllActivated.Invoke();
            if (SlideBallInputs.AnyShiftUp())
                SendToTeamActivated.Invoke();
            if (SlideBallInputs.AnyEnterDown())
            {
                if (input.gameObject.activeSelf)
                    SendContent(input.text, SlideBallInputs.AnyShift());
                if (hideWhenNotActive)
                {
                    ActivateInput(!input.gameObject.activeSelf);
                }
            }
            if (input.gameObject.activeSelf)
            {
                input.Select();
                input.ActivateInputField();
            }
        }
    }

    private void ActivateInput(bool active)
    {
        input.gameObject.SetActive(active);
        if (input.gameObject.activeSelf)
            SlideBallInputs.currentPart = SlideBallInputs.GUIPart.CHAT;
        else
            SlideBallInputs.currentPart = Scenes.CurrentSceneDefaultGUIPart();
    }

    public void SendContent(string content, bool sendToAll)
    {
        MyComponents.ChatManager.UserWriteMessage(content, sendToAll);
        input.text = "";
    }
}
