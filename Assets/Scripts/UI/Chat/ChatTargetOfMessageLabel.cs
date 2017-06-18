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

    void OnEnable()
    {
        writer.ChangeContext += ChangeContext;
    }

    void OnDisable()
    {
        writer.ChangeContext -= ChangeContext;
    }

    public void ChangeContext()
    {
        if (text.text == AllLabel)
            text.text = TeamLabel;
        else if (text.text == TeamLabel)
            text.text = AllLabel;
    }
}
