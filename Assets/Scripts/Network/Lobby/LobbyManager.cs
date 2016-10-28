using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{

    public GameObject MainPanel;
    public GameObject RoomListPanel;
    public RoomManager RoomManager;

    public GameObject NoServerFound;

    public GameObject serverInfoPrefab;

    public bool StartGameImmediately;

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
                    RoomListPanel.SetActive(false);
                    RoomManager.gameObject.SetActive(false);
                    break;

                case State.ROOMLIST:
                    MainPanel.SetActive(false);
                    RoomListPanel.SetActive(true);
                    RoomManager.gameObject.SetActive(false);
                    break;
                case State.ROOM:
                    MainPanel.SetActive(false);
                    RoomListPanel.SetActive(false);
                    RoomManager.gameObject.SetActive(true);
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
    private string[] roomList = new string[0];
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
        MyGameObjects.NetworkManagement.ConnectedToRoom += ListanToBufferedMessages;
    }

    protected void OnDisable()
    {
        MyGameObjects.NetworkManagement.RoomCreated -= RoomState;
        MyGameObjects.NetworkManagement.RoomCreated -= CreatePlayerInfo;
        MyGameObjects.NetworkManagement.ConnectedToRoom -= RoomState;
        MyGameObjects.NetworkManagement.ConnectedToRoom -= ListanToBufferedMessages;
        MyGameObjects.NetworkManagement.ReceivedAllBufferedMessages -= CreatePlayerInfo;

    }

    private void CreatePlayerInfo()
    {
        RoomManager.CreateMyPlayerInfo();
    }

    private void RoomState()
    {
        MyState = State.ROOM;
    }

    private void ListanToBufferedMessages()
    {
        MyGameObjects.NetworkManagement.ReceivedAllBufferedMessages += CreatePlayerInfo;
    }

    void ConnectToDefaultRoom()
    {
        MyGameObjects.NetworkManagement.ConnectToRoom(DEFAULT_ROOM);
    }

    public void UpdateRoomList(string[] rooms)
    {
        Debug.Log("UpdateRoomList " + rooms.Length);
        roomList = rooms;
        MyState = State.ROOMLIST;
        if (roomList.Length == 0)
        {
            NoServerFound.SetActive(true);
        }
        else
        {
            NoServerFound.SetActive(false);
            int y = -90;
            foreach (string room in roomList)
            {
                GameObject go = Instantiate(serverInfoPrefab);
                go.transform.SetParent(RoomListPanel.transform, false);
                go.GetComponent<ServerInfoPanel>().SetRoomName(room);
                go.GetComponent<RectTransform>().localPosition += y * new Vector3(0, 1, 0);
                y -= 50;
            }
        }
        if (StartGameImmediately)
        {
            if (roomList.Length == 0)
            {
                MyGameObjects.NetworkManagement.CreateRoom("DefaultRoom");
            }
            else
            {
                MyGameObjects.NetworkManagement.ConnectToRoom("DefaultRoom");
            }
        }
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

}