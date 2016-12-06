using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class VictoryUI : MonoBehaviour
{

    public Text text;

    public void ReturnToRoom()
    {
        MyComponents.ResetNetworkComponents();
    }

    public void SetVictoryText(Team team)
    {
        text.text = Language.Instance.texts["Won_Match"].Replace("%s", Language.Instance.texts[Teams.GetTeamNameKey(team)]);
        text.color = Colors.Teams[(int)team];
    }
}
