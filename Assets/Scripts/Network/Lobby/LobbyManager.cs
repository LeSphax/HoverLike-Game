using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{

    public GameObject MainPanel;
    public RoomListPanel RoomListPanel;
    public RoomManager RoomManager;

    public bool StartGameImmediately;
    public int NumberPlayersToStartGame;

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
                    Host.text = MyComponents.NetworkManagement.RoomName;
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
            MyComponents.NetworkManagement.ServerStartFailed += InvokeConnectToDefaultRoom;
            MyComponents.NetworkManagement.CreateRoom(DEFAULT_ROOM);
            InvokeRepeating("CheckStartGame", 0f, 0.2f);
        }
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
        MyComponents.NetworkManagement.RoomCreated += RoomState;
        MyComponents.NetworkManagement.RoomCreated += CreatePlayerInfo;
        MyComponents.NetworkManagement.ConnectedToRoom += RoomState;
        MyComponents.NetworkManagement.ConnectedToRoom += ListenToBufferedMessages;
    }

    protected void OnDisable()
    {
        if (MyComponents.NetworkManagement != null)
        {
            MyComponents.NetworkManagement.RoomCreated -= RoomState;
            MyComponents.NetworkManagement.RoomCreated -= CreatePlayerInfo;
            MyComponents.NetworkManagement.ConnectedToRoom -= RoomState;
            MyComponents.NetworkManagement.ConnectedToRoom -= ListenToBufferedMessages;
        }
    }

    private void CreatePlayerInfo()
    {
        MyComponents.NetworkManagement.ReceivedAllBufferedMessages -= CreatePlayerInfo;
        RoomManager.CreateMyPlayerInfo();
    }

    private void RoomState()
    {
        MyState = State.ROOM;
    }

    private void ListenToBufferedMessages()
    {
        MyComponents.NetworkManagement.ReceivedAllBufferedMessages += CreatePlayerInfo;
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

    public void StartGame()
    {
        MyComponents.NetworkManagement.BlockRoom();
        //Debug.LogWarning("There could be a problem if a player connects before the block room message reaches the signaling server");
        MyComponents.GameInitialization.StartGame();
    }

    public void UpdateRoomList(string[] list)
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