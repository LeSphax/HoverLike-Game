using UnityEngine;
using System.Runtime.InteropServices;

public class LinkPanel : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern string ShowLink(string roomName);

    public void ShowLinkToRoom()
    {
        ShowLink(((NetworkManagement)MyComponents.NetworkManagement).RoomName);
    }


}
