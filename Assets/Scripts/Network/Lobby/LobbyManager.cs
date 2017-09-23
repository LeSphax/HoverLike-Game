using Navigation;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{

    public GameObject MainPanel;
    public TopPanel topPanel;
    public RoomListPanel RoomListPanel;

    private string HeaderRoomName;

    private enum State
    {
        IDLE,
        ROOMLIST,
        SETTINGS,
    }

    private State state;
    private State MyState
    {
        get
        {
            return state;
        }
        set
        {
            switch (value)
            {
                case State.IDLE:
                    //MyComponents.PopUp.ClosePopUp();
                    MainPanel.SetActive(true);
                    //
                    RoomListPanel.gameObject.SetActive(false);
                    RoomListPanel.Reset();
                    //
                    topPanel.Status = "In Lobby";
                    break;
                case State.ROOMLIST:
                    MyComponents.PopUp.ClosePopUp();
                    MainPanel.SetActive(false);
                    //
                    RoomListPanel.gameObject.SetActive(true);
                    //
                    topPanel.Status = "Looking for Game";
                    break;
                case State.SETTINGS:
                    MyComponents.PopUp.ClosePopUp();
                    MainPanel.SetActive(false);
                    //
                    RoomListPanel.gameObject.SetActive(false);
                    //
                    topPanel.Status = "Setting Settings int the Settings Panel";
                    break;
            }
            state = value;
        }
    }

    public Text inputField;
    private const string SERVER_ROOM = "Server Room ";
    private static int serverRoomIndex = 0;

    protected void Awake()
    {
        MyState = State.IDLE;
        topPanel.RoomName = "";
        AudioListener.volume = UserSettings.Volume;
    }

    private void Start()
    {
        if (RequestParameters.HasKey("RoomName"))
        {
            HeaderRoomName = RequestParameters.GetAndRemoveValue("RoomName");
            MyComponents.NetworkManagement.ConnectToRoom(HeaderRoomName);
            MyComponents.NetworkManagement.ConnectionFailed += CreateHeaderRoom;
        }
        else
        {
            bool editor = false;
#if UNITY_EDITOR
            editor = true;
#endif
            Debug.Log("Before Join room immediately " + EditorVariables.JoinRoomImmediately);

            if (EditorVariables.JoinRoomImmediately)
            {
                Debug.Log("Join room immediately " + editor + "  " + EditorVariables.EditorIsServer);
                if (editor == EditorVariables.EditorIsServer)
                {
                    Debug.Log("Create the room");
                    if (EditorVariables.HeadlessServer)
                    {
                        MyComponents.NetworkManagement.ServerStartFailed += CreateServerRoom;
                        Debug.Log("Headless Server");
                    }
                    else
                        MyComponents.NetworkManagement.ServerStartFailed += ConnectToDefaultRoom;
                    CreateServerRoom();
                }
                else
                {
                    MyComponents.NetworkManagement.ConnectionFailed += ConnectToDefaultRoom;
                    ConnectToDefaultRoom();
                }
            }
        }
    }

    private static void CreateServerRoom()
    {
        serverRoomIndex++;
        MyComponents.NetworkManagement.CreateRoom(SERVER_ROOM + serverRoomIndex);
    }


    private void CreateHeaderRoom()
    {
        Debug.Log("CreateHeaderRoom " + HeaderRoomName);
        MyComponents.NetworkManagement.ConnectionFailed -= CreateHeaderRoom;
        MyComponents.NetworkManagement.CreateRoom(HeaderRoomName);
    }

    private void ConnectToDefaultRoom()
    {
        MyComponents.NetworkManagement.ServerStartFailed -= ConnectToDefaultRoom;
        MyComponents.NetworkManagement.ConnectToRoom(SERVER_ROOM);
    }

    protected void OnEnable()
    {
        MyComponents.NetworkManagement.RoomCreated += InitGame;
        MyComponents.NetworkManagement.ConnectedToRoom += InitGame;

        topPanel.BackPressed += GoBack;
    }

    protected void OnDisable()
    {
        if (MyComponents.NetworkManagement != null)
        {
            MyComponents.NetworkManagement.ConnectionFailed -= ConnectToDefaultRoom;
            MyComponents.NetworkManagement.ServerStartFailed -= ConnectToDefaultRoom;
            MyComponents.NetworkManagement.RoomCreated -= InitGame;
            MyComponents.NetworkManagement.ConnectedToRoom -= InitGame;
            topPanel.BackPressed -= GoBack;
        }
    }

    private void InitGame()
    {
        MyComponents.GameInitialization.InitGame();
    }

    public void CreateGame()
    {
        MyComponents.NetworkManagement.CreateRoom(inputField.text);
    }

    public void JoinGame()
    {
        MyComponents.NetworkManagement.ConnectToRoom(inputField.text);
    }

    public void RefreshServers()
    {
        if (MyState == State.ROOMLIST)
        {
            ListServers();
        }
    }

    public void ListServers()
    {
        MyComponents.NetworkManagement.GetRooms();
        MyComponents.PopUp.Show(Language.Instance.texts["Connecting"]);
    }

    public void UpdateRoomList(List<RoomData> list)
    {
        MyState = State.ROOMLIST;
        RoomListPanel.UpdateRoomList(list);
    }

    public void GoBack()
    {
        MyComponents.ResetNetworkComponents();
        Reset();
    }

    internal void Reset()
    {
        MyState = State.IDLE;
    }
}