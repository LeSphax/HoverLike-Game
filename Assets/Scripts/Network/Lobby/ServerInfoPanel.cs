using UnityEngine;
using UnityEngine.UI;

public class ServerInfoPanel : MonoBehaviour
{

    public Text roomNameLabel;
    public Text nbPlayersLabel;
    public Button joinButton;
    public GameObject lockIcon;

    private RoomData data;


    public void JoinRoom()
    {
        if (data.hasPassword)
        {
            PasswordPanel.InstantiatePanel(data.name);
        }
        else
            ((NetworkManagement) MyComponents.NetworkManagement).ConnectToRoom(roomNameLabel.text);
    }

    public void SetRoomData(RoomData data)
    {
        this.data = data;
        roomNameLabel.text = data.gameStarted ? data.name + " (In Match)" : data.name;
        nbPlayersLabel.text = data.nbPlayers + "/8";
        joinButton.interactable = !data.gameStarted;
        if (!data.hasPassword)
            lockIcon.SetActive(false);
    }
}
