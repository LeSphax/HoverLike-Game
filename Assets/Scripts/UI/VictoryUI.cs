using UnityEngine;
using UnityEngine.UI;

public class VictoryUI : MonoBehaviour
{
    public GameObject panel;
    public Text text;
    public MenuInGame menu;

    private void Start()
    {
        panel.SetActive(false);
    }

    public void ReturnToRoom()
    {
        menu.ReturnToRoom();
    }

    public void SetVictoryText(Team team)
    {
        panel.SetActive(true);
        text.text = Language.Instance.texts["Won_Match"].Replace("%s", Language.Instance.texts[Teams.GetTeamNameKey(team)]);
        text.color = Colors.Teams[(int)team];
    }
}
