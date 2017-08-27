using UnityEngine;
using UnityEngine.UI;

public class ChatTargetOfMessageLabel : MonoBehaviour
{
    private const string AllLabel = "[ALL] :";
    private const string TeamLabel = "[TEAM] :";
    public ChatMessageWriter writer;
    public Text text;

    public bool SendToAll
    {
        get
        {
            return text.text == AllLabel;
        }
    }

    public void SetTarget(TargetOfMessage target)
    {
        switch (target)
        {
            case TargetOfMessage.ALL:
                text.text = AllLabel;
                break;
            case TargetOfMessage.TEAM:
                text.text = TeamLabel;
                break;
            default:
                throw new UnhandledSwitchCaseException(target);
        }
    }
}
