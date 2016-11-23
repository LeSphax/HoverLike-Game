using System.Runtime.InteropServices;
using UnityEngine;

public class JavascriptAPI : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern string GetRoomName();

    [DllImport("__Internal")]
    private static extern void UnityReady();

    protected void Awake()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        WebGLInput.captureAllKeyboardInput = false; 
        UnityReady();
#endif
    }

    public static void TryConnectToRoom()
    {
        string roomName = GetRoomName();
        if (roomName != "NoRoomName")
        {
            MyComponents.NetworkManagement.ConnectToRoom(roomName);
        }
    }

    public void SetNickname(string nickname)
    {
        NicknamePanel.nickname = nickname;
        TryConnectToRoom();
#if !UNITY_EDITOR && UNITY_WEBGL
            WebGLInput.captureAllKeyboardInput = true; 
#endif
    }
}