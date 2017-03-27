using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ChatTargetOfMessageLabel : MonoBehaviour {

    public ChatMessageWriter writer;
    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    void OnEnable()
    {
        writer.SendToAllActivated += SendToAll;
        writer.SendToTeamActivated += SendToTeam;
    }

    void OnDisable()
    {
        writer.SendToAllActivated -= SendToAll;
        writer.SendToTeamActivated -= SendToTeam;
    }

    void SendToAll()
    {
        text.text = "[ALL] :";
    }

    void SendToTeam()
    {
        text.text = "[TEAM] :";
    }
}
