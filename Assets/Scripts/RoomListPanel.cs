using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomListPanel : MonoBehaviour {

    private string[] roomList = new string[0];

    public GameObject NoServerFound;
    public GameObject serverInfoPrefab;

    private List<GameObject> serverInfos = new List<GameObject>();


    public void UpdateRoomList(string[] rooms)
    {
        Debug.Log("UpdateRoomList " + rooms.Length);
        roomList = rooms;
        if (roomList.Length == 0)
        {
            NoServerFound.SetActive(true);
        }
        else
        {
            NoServerFound.SetActive(false);
            int y = -90;
            foreach (string room in roomList)
            {
                GameObject go = Instantiate(serverInfoPrefab);
                serverInfos.Add(go);
                go.transform.SetParent(transform, false);
                go.GetComponent<ServerInfoPanel>().SetRoomName(room);
                go.GetComponent<RectTransform>().localPosition += y * new Vector3(0, 1, 0);
                y -= 50;
            }
        }
    }

    public void Reset()
    {
        foreach(GameObject serverInfo in serverInfos)
        {
            Destroy(serverInfo);
        }
        serverInfos.Clear();
    }
}
