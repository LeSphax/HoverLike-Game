using Navigation;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{

    public GameObject MainPanel;
    public TopPanel topPanel;
    public RoomListPanel RoomListPanel;

    public bool StartGameImmediately;
    public int NumberPlayersToStartGame;

    private enum State
    {
        IDLE,
        ROOMLIST,
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
                    MainPanel.SetActive(true);
                    //
                    RoomListPanel.gameObject.SetActive(false);
                    RoomListPanel.Reset();
                    //
                    topPanel.Status = "In Lobby";
                    break;
                case State.ROOMLIST:
                    MainPanel.SetActive(false);
                    //
                    RoomListPanel.gameObject.SetActive(true);
                    //
                    topPanel.Status = "Looking for Game";
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
        if (StartGameImmediately)
        {
            MyComponents.NetworkManagement.ServerStartFailed += InvokeConnectToDefaultRoom;
            MyComponents.NetworkManagement.CreateRoom(DEFAULT_ROOM);
            InvokeRepeating("CheckStartGame", 0f, 0.2f);
        }
        topPanel.RoomName = "";
    }

    private void InvokeConnectToDefaultRoom()
    {
        MyComponents.NetworkManagement.ServerStartFailed -= InvokeConnectToDefaultRoom;
        ConnectToDefaultRoom();
    }

    void CheckStartGame()
    {
        if (MyComponents.NetworkManagement.isServer && (MyComponents.NetworkManagement.GetNumberPlayers() == NumberPlayersToStartGame || NumberPlayersToStartGame == 1))
        {
            Invoke("StartGame", 1f);
            CancelInvoke("CheckStartGame");
        }
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
            topPanel.BackPressed += GoBack;
        }
    }

    private void LoadRoom()
    {
        NavigationManager.LoadScene(Scenes.Room, false, false);
    }

    void ConnectToDefaultRoom()
    {
        MyComponents.NetworkManagement.ConnectToRoom(DEFAULT_ROOM);
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
    }

    internal void Reset()
    {
        MyState = State.IDLE;
    }
}