using UnityEngine;
using UnityEngine.UI;

public class VictoryUI : MonoBehaviour
{
    public GameObject panel;
    public Text text;
    public MenuInGame menu;
    public Button playAgainButton;
    public Text clientText;



    private void Start()
    {
        Show(false);
    }

    public void PlayAgain()
    {
        MyComponents.VictoryPose.StopVictoryPose();
    }

    public void Show(bool show)
    {
        panel.SetActive(show);
    }

    public void SetVictoryText(Team team)
    {
        Show(true);
        text.text = Language.Instance.texts["Won_Match"].Replace("%s", Language.Instance.texts[Teams.GetTeamNameKey(team)]);
        text.color = Colors.Teams[(int)team];

        playAgainButton.gameObject.SetActive( MyComponents.NetworkManagement.IsServer);
        clientText.gameObject.SetActive(!MyComponents.NetworkManagement.IsServer);
    }
}
