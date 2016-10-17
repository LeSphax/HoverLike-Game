using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    private string[] roomList = new string[0];
    public Text inputField;

    public void UpdateRoomList(string[] rooms)
    {
        roomList = rooms;
    }

    void onGUI()
    {
        GUI.Label(new Rect(500, 0, 150, 50), "ROOMS");
        int y = 60;
        foreach (string room in roomList)
        {
            if (GUI.Button(new Rect(350, y, 300, 50), room))
            {
                MyGameObjects.NetworkManagement.ConnectToRoom(room);
            }
            y += 60;
        }
        if (GUI.Button(new Rect(10, 60, 300, 50), "CreateNewRoom"))
        {
            MyGameObjects.NetworkManagement.CreateRoom(inputField.text);
        }
    }
}