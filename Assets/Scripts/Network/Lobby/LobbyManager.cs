using Navigation;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{

    public GameObject MainPanel;
    public TopPanel topPanel;
    public RoomListPanel RoomListPanel;

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
                    MyComponents.PopUp.ClosePopUp();
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
    private const string DEFAULT_ROOM = "Da Room";

    protected void Awake()
    {
        MyState = State.IDLE;
        if (EditorVariables.StartGameImmediately)
        {
            MyComponents.NetworkManagement.ServerStartFailed += ConnectToDefaultRoom;
            MyComponents.NetworkManagement.CreateRoom(DEFAULT_ROOM);
        }
        topPanel.RoomName = "";
    }

    private void Start()
    {
        if (RequestParameters.HasKey("RoomName"))
        {
            MyComponents.NetworkManagement.ConnectToRoom(RequestParameters.GetValue("RoomName"));
        }
    }

    private void ConnectToDefaultRoom()
    {
        MyComponents.NetworkManagement.ServerStartFailed -= ConnectToDefaultRoom;
        MyComponents.NetworkManagement.ConnectToRoom(DEFAULT_ROOM);
    }

    protected void OnEnable()
    {
        MyComponents.NetworkManagement.RoomCreated += LoadRoom;
        MyComponents.NetworkManagement.ConnectedToRoom += LoadRoom;
        topPanel.BackPressed += GoBack;
    }

    protected void OnDisable()
    {
        if (MyComponents.NetworkManagement != null)
        {
            MyComponents.NetworkManagement.RoomCreated -= LoadRoom;
            MyComponents.NetworkManagement.ConnectedToRoom -= LoadRoom;
            topPanel.BackPressed -= GoBack;
        }
    }

    private void LoadRoom()
    {
        NavigationManager.LoadScene(Scenes.Room, false, false);
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
        Reset();
        MyComponents.ResetNetworkComponents();
        MyState = State.IDLE;
    }

    internal void Reset()
    {
        MyState = State.IDLE;
    }
}