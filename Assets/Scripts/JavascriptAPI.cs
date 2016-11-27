using System.Runtime.InteropServices;
using UnityEngine;

public class JavascriptAPI : MonoBehaviour
{
    public static string nickname = "Focus Graves";

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
        JavascriptAPI.nickname = nickname;
    }

    public void SetConfiguration(string inputs)
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            Debug.LogError(inputs[i]);
            Inputs.Keys[i] = inputs[i].ToString();
        }
        TryConnectToRoom();
#if !UNITY_EDITOR && UNITY_WEBGL
            WebGLInput.captureAllKeyboardInput = true; 
#endif
    }
}