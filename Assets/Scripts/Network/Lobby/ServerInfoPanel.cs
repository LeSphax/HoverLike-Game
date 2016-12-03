using UnityEngine;
using UnityEngine.UI;

public class ServerInfoPanel : MonoBehaviour {

    public Text roomNameLabel;
    public Text nbPlayersLabel;


    public void JoinRoom()
    {
        MyComponents.NetworkManagement.ConnectToRoom(roomNameLabel.text);
    }

    public void SetRoomData(RoomData data)
    {
        roomNameLabel.text = data.name;
        nbPlayersLabel.text = data.nbPlayers + "/8";
    }
}
