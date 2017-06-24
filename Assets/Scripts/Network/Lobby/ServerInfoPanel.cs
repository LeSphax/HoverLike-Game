using UnityEngine;
using UnityEngine.UI;

public class ServerInfoPanel : MonoBehaviour {

    public Text roomNameLabel;
    public Text nbPlayersLabel;
    public Button joinButton;


    public void JoinRoom()
    {
        MyComponents.NetworkManagement.ConnectToRoom(roomNameLabel.text);
    }

    public void SetRoomData(RoomData data)
    {
        roomNameLabel.text = data.gameStarted ? data.name + " (In Match)" : data.name;
        nbPlayersLabel.text = data.nbPlayers + "/8";
        joinButton.interactable = !data.gameStarted;
    }
}
