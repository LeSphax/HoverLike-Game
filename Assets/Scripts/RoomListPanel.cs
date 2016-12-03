using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomListPanel : MonoBehaviour {

    private List<RoomData> roomList = new List<RoomData>();

    public GameObject NoServerFound;
    public GameObject serverInfoPrefab;

    private List<GameObject> serverInfos = new List<GameObject>();


    public void UpdateRoomList(List<RoomData> rooms)
    {
        roomList = rooms;
        if (roomList.Count == 0)
        {
            NoServerFound.SetActive(true);
        }
        else
        {
            NoServerFound.SetActive(false);
            int y = -90;
            foreach (RoomData room in roomList)
            {
                GameObject go = Instantiate(serverInfoPrefab);
                serverInfos.Add(go);
                go.transform.SetParent(transform, false);
                go.GetComponent<ServerInfoPanel>().SetRoomData(room);
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
