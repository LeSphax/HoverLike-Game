using PlayerManagement;
using UnityEngine;
using UnityEngine.Assertions;

public class RoomManager : MonoBehaviour
{
    public TopPanel topPanel;
    public GameObject[] teamPanels;
    public GameObject StartButton;

    private static RoomManager instance;
    public static RoomManager Instance
    {
        get
        {
            Assert.IsTrue(Scenes.IsCurrentScene(Scenes.RoomIndex));
            if (instance == null)
            {
                instance = GameObject.FindGameObjectWithTag(Tags.Room).GetComponent<RoomManager>();
            }
            return instance;
        }
    }

    public void OnEnable()
    {
        if (MyComponents.NetworkManagement.isServer)
        {
            StartButton.SetActive(true);
        }
        else
        {
            StartButton.SetActive(false);
        }
        MyComponents.NetworkManagement.ReceivedAllBufferedMessages += CreateMyPlayerInfo;
    }

    public void OnDisable()
    {
        MyComponents.NetworkManagement.ReceivedAllBufferedMessages -= CreateMyPlayerInfo;
    }

    private void Start()
    {
        topPanel.Status = "In Game Room";
        topPanel.RoomName = MyComponents.NetworkManagement.RoomName;
        topPanel.BackPressed += GoBack;
        if (EditorVariables.StartGameImmediately)
        {
            InvokeRepeating("CheckStartGame", 0f, 0.2f);
        }
    }

    void CheckStartGame()
    {
        if (MyComponents.NetworkManagement.isServer && (MyComponents.NetworkManagement.GetNumberPlayers() == EditorVariables.NumberPlayersToStartGame || EditorVariables.NumberPlayersToStartGame == 1))
        {
            Invoke("StartGame", 1f);
            CancelInvoke("CheckStartGame");
        }
    }

    public void StartGame()
    {
        MyComponents.NetworkManagement.BlockRoom();
        //Debug.LogWarning("There could be a problem if a player connects before the block room message reaches the signaling server");
        MyComponents.GameInitialization.StartGame();
    }

    public void PutPlayerInTeam(PlayerInfo player, Team team)
    {
        player.transform.SetParent(teamPanels[(int)team].transform, false);
    }

    public void ChangeTeam(int teamNumber)
    {
        Players.MyPlayer.Team = (Team)teamNumber;
    }

    public void CreateMyPlayerInfo()
    {
        Debug.LogError("CreateMyPlayerInfo " + Players.MyPlayer.id);
        if (Players.MyPlayer.gameobjectAvatar == null)
            Players.MyPlayer.gameobjectAvatar = MyComponents.NetworkViewsManagement.Instantiate(Paths.PLAYER_INFO, Vector3.zero, Quaternion.identity, Players.MyPlayer.id);
    }

    public void Reset()
    {
        foreach (GameObject teamPanel in teamPanels)
        {
            foreach (Transform child in teamPanel.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void GoBack()
    {
        MyComponents.ResetNetworkComponents();
    }

    protected void OnDestroy()
    {
        instance = null;
    }
}