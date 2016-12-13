using System.Runtime.InteropServices;
using UnityEngine;

public class JavascriptAPI : MonoBehaviour
{
    public static string nickname = "Focus Graves";
    public static bool isFirstGame = false;

    [DllImport("__Internal")]
    private static extern string GetRoomName();

    [DllImport("__Internal")]
    private static extern void UnityReady();

    protected void Awake()
    {
        nickname = Functions.RandomString(10);
#if !UNITY_EDITOR && UNITY_WEBGL
        isFirstGame = true;
        WebGLInput.captureAllKeyboardInput = false; 
        UnityReady();
#endif
#if !UNITY_WEBGL || UNITY_EDITOR
        isFirstGame = false;
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
            Inputs.Keys[i] = inputs[i].ToString();
        }
        TryConnectToRoom();
#if !UNITY_EDITOR && UNITY_WEBGL
            WebGLInput.captureAllKeyboardInput = true; 
#endif
    }

    public void IsNotFirstGame()
    {
        isFirstGame = false;
    }

}