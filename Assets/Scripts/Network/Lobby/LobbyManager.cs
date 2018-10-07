using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : SlideBall.MonoBehaviour
{

    public GameObject MainPanel;
    public TopPanel topPanel;
    public RoomListPanel RoomListPanel;
    private NetworkManagement mNetworkManagement;

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
        mNetworkManagement = (NetworkManagement) MyComponents.NetworkManagement;
    }

    private void Start()
    {
        if (RequestParameters.HasKey("RoomName"))
        {
            HeaderRoomName = RequestParameters.GetAndRemoveValue("RoomName");
            mNetworkManagement.ConnectToRoom(HeaderRoomName);
            mNetworkManagement.ConnectionFailed += CreateHeaderRoom;
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
                        mNetworkManagement.ServerStartFailed += CreateServerRoom;
                        Debug.Log("Headless Server");
                    }
                    else
                        mNetworkManagement.ServerStartFailed += ConnectToDefaultRoom;
                    CreateServerRoom();
                }
                else
                {
                    mNetworkManagement.ConnectionFailed += ConnectToDefaultRoom;
                    ConnectToDefaultRoom();
                }
            }
        }
    }

    private void CreateServerRoom()
    {
        serverRoomIndex++;
        mNetworkManagement.CreateRoom(SERVER_ROOM + serverRoomIndex);
    }


    private void CreateHeaderRoom()
    {
        Debug.Log("CreateHeaderRoom " + HeaderRoomName);
        mNetworkManagement.ConnectionFailed -= CreateHeaderRoom;
        mNetworkManagement.CreateRoom(HeaderRoomName);
    }

    private void ConnectToDefaultRoom()
    {
        mNetworkManagement.ServerStartFailed -= ConnectToDefaultRoom;
        mNetworkManagement.ConnectToRoom(SERVER_ROOM + 1);
    }

    protected void OnEnable()
    {
        mNetworkManagement.RoomCreated += InitGame;
        mNetworkManagement.ConnectedToRoom += InitGame;

        topPanel.BackPressed += GoBack;
    }

    protected void OnDisable()
    {
        if (mNetworkManagement != null)
        {
            mNetworkManagement.ConnectionFailed -= ConnectToDefaultRoom;
            mNetworkManagement.ServerStartFailed -= ConnectToDefaultRoom;
            mNetworkManagement.RoomCreated -= InitGame;
            mNetworkManagement.ConnectedToRoom -= InitGame;
            topPanel.BackPressed -= GoBack;
        }
    }

    private void InitGame()
    {
        MyComponents.GameState.InitGame();
    }

    public void CreateGame()
    {
        mNetworkManagement.CreateRoom(inputField.text);
    }

    public void JoinGame()
    {
        mNetworkManagement.ConnectToRoom(inputField.text);
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
        mNetworkManagement.GetRooms();
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