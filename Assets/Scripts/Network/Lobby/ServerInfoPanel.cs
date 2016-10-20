using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ServerInfoPanel : MonoBehaviour {

    public Text roomNameLabel;


	public void JoinRoom()
    {
        MyGameObjects.NetworkManagement.ConnectToRoom(roomNameLabel.text);
    }

    public void SetRoomName(string name)
    {
        roomNameLabel.text = name;
    }
}
