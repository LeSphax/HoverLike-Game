using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{

    public GameObject MainPanel;
    public RoomListPanel RoomListPanel;
    public RoomManager RoomManager;

    public bool StartGameImmediately;

    public Text Status;
    public Text Host;

    private enum State
    {
        IDLE,
        ROOMLIST,
        ROOM,
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
                    RoomManager.gameObject.SetActive(false);
                    RoomManager.Reset();
                    //
                    Status.text = "In Lobby";
                    break;

                case State.ROOMLIST:
                    MainPanel.SetActive(false);
                    //
                    RoomListPanel.gameObject.SetActive(true);
                    //
                    RoomManager.gameObject.SetActive(false);
                    RoomManager.Reset();
                    //
                    Status.text = "Looking for Game";
                    break;
                case State.ROOM:
                    MainPanel.SetActive(false);
                    //
                    RoomListPanel.gameObject.SetActive(false);
                    RoomListPanel.Reset();
                    //
                    RoomManager.gameObject.SetActive(true);
                    //
                    Status.text = "In Game Room";
                    break;
            }
            state = value;
        }
    }

    public bool IsInRoom
    {
        get
        {
            return MyState == State.ROOM;
        }
    }
    public Text inputField;
    private const string DEFAULT_ROOM = "Da Room";

    protected void Awake()
    {
        MyState = State.IDLE;
        if (StartGameImmediately)
        {
            MyGameObjects.NetworkManagement.ServerStartFailed += () => Invoke("ConnectToDefaultRoom", 0.1f);
            MyGameObjects.NetworkManagement.CreateRoom(DEFAULT_ROOM);
        }
    }

    protected void OnEnable()
    {
        MyGameObjects.NetworkManagement.RoomCreated += RoomState;
        MyGameObjects.NetworkManagement.RoomCreated += CreatePlayerInfo;
        MyGameObjects.NetworkManagement.ConnectedToRoom += RoomState;
        MyGameObjects.NetworkManagement.ConnectedToRoom += ListenToBufferedMessages;
    }

    protected void OnDisable()
    {
        if (MyGameObjects.NetworkManagement != null)
        {
            MyGameObjects.NetworkManagement.RoomCreated -= RoomState;
            MyGameObjects.NetworkManagement.RoomCreated -= CreatePlayerInfo;
            MyGameObjects.NetworkManagement.ConnectedToRoom -= RoomState;
            MyGameObjects.NetworkManagement.ConnectedToRoom -= ListenToBufferedMessages;
        }
    }

    private void CreatePlayerInfo()
    {
        MyGameObjects.NetworkManagement.ReceivedAllBufferedMessages -= CreatePlayerInfo;
        RoomManager.CreateMyPlayerInfo();
    }

    private void RoomState()
    {
        MyState = State.ROOM;
    }

    private void ListenToBufferedMessages()
    {
        MyGameObjects.NetworkManagement.ReceivedAllBufferedMessages += CreatePlayerInfo;
    }

    void ConnectToDefaultRoom()
    {
        MyGameObjects.NetworkManagement.ConnectToRoom(DEFAULT_ROOM);
    }

    public void CreateGame()
    {
        MyGameObjects.NetworkManagement.CreateRoom(inputField.text);
    }

    public void ListServers()
    {
        MyGameObjects.NetworkManagement.GetRooms();
    }

    public void StartGame()
    {
        MyGameObjects.GameInitialization.StartGame();
    }

    public void UpdateRoomList(string[] list)
    {
        MyState = State.ROOMLIST;
        RoomListPanel.UpdateRoomList(list);
        if (StartGameImmediately)
        {
            if (list.Length == 0)
            {
                MyGameObjects.NetworkManagement.CreateRoom("DefaultRoom");
            }
            else
            {
                MyGameObjects.NetworkManagement.ConnectToRoom("DefaultRoom");
            }
        }
    }

    public void GoBack()
    {
        MyState = State.IDLE;
        MyGameObjects.NetworkManagement.Reset();
    }
}