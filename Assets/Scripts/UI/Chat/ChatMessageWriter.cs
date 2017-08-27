using UnityEngine;
using UnityEngine.UI;

public class ChatMessageWriter : MonoBehaviour
{
    public InputField input;
    public ChatTargetOfMessageLabel target;

    private void Awake()
    {
        ActivateInput(false);
    }

    private void Update()
    {
        if (SlideBallInputs.AnyEnterDown())
        {
            if (input.gameObject.activeSelf)
                SendContent(input.text, target.SendToAll);
            else
            {
                target.SetTarget(SlideBallInputs.AnyShift() ? TargetOfMessage.ALL : TargetOfMessage.TEAM);
                ActivateInput(true);
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
        if (content.Length >= 4)
        {
            if (content.Substring(0, 4).Equals("/all") || content.Substring(0, 4).Equals("\all"))
            {
                sendToAll = true;
                content = content.Substring(4);
            }
        }
        if (content != "")
        {
            MyComponents.ChatManager.UserWriteMessage(content, sendToAll);
        }
        ActivateInput(false);
        input.text = "";
    }
}
