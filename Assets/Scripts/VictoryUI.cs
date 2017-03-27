using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class VictoryUI : MonoBehaviour
{
    public GameObject panel;
    public Text text;

    private void Start()
    {
        panel.SetActive(false);
    }

    public void ReturnToRoom()
    {
        MyComponents.ResetGameComponents();
        MyComponents.ResetScene();
    }

    public void SetVictoryText(Team team)
    {
        panel.SetActive(true);
        text.text = Language.Instance.texts["Won_Match"].Replace("%s", Language.Instance.texts[Teams.GetTeamNameKey(team)]);
        text.color = Colors.Teams[(int)team];
    }
}
