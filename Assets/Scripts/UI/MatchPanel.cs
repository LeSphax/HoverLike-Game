using Byn.Net;
using PlayerManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchPanel : MonoBehaviour
{
    public Transform[] teamPanels;
    public GameObject StartButton;
    public Button[] teamButtons;

    public static int MaxNumberPlayers = 7;

    public static string password = "";
    public static string Password
    {
        get
        {
            return password;
        }
        set
        {
            password = value;
            MyComponents.NetworkManagement.RefreshRoomData();
        }
    }

    private List<GameObject> playerInfos = new List<GameObject>();

    private bool isPlaying;

    public void Open(bool open)
    {
        gameObject.SetActive(open);
        if (open)
        {
            RefreshPlayerInfos(ConnectionId.INVALID);
        }
    }

    public void Reset()
    {
        foreach (GameObject playerInfo in playerInfos)
        {
            Destroy(playerInfo);
        }
    }

    private void RefreshPlayerInfos(ConnectionId id)
    {
        Reset();
        foreach (Player player in Players.players.Values)
        {
            PlayerInfo playerInfo = Instantiate(ResourcesGetter.PlayerInfoPrefab).GetComponent<PlayerInfo>();
            playerInfos.Add(playerInfo.gameObject);
            playerInfo.TeamChanged += SetPlayerInTeamPanel;
            playerInfo.MyPlayer = player;
        }
    }

    private void SetPlayerInTeamPanel(PlayerInfo playerInfo, Team team)
    {
        if (team == Team.NONE)
            return;
        playerInfo.transform.SetParent(teamPanels[(int)team], false);
        CheckTeamButtons();
    }

    private void CheckTeamButtons()
    {
        if (!isPlaying)
        {
            if (Players.GetPlayersInTeam(Team.BLUE).Count < 4)
            {
                teamButtons[0].interactable = true;
            }
            else
            {
                teamButtons[0].interactable = false;
            }
            if (Players.GetPlayersInTeam(Team.RED).Count < 4)
            {
                teamButtons[1].interactable = true;
            }
            else
            {
                teamButtons[1].interactable = false;
            }
        }
        else
        {
            teamButtons.ForEach(b => b.interactable = !isPlaying);
        }
    }

    private void Awake()
    {
        if (EditorVariables.HeadlessServer)
            Destroy(this);
    }

    private void Start()
    {
        isPlaying = false;
        MyComponents.GameState.MatchStartOrEnd += SetPlayingState;

        if (EditorVariables.StartGameImmediately)
        {
            InvokeRepeating("CheckStartGame", 0f, 0.2f);
        }
        Players.NewPlayerCreated += RefreshPlayerInfos;
    }

    void CheckStartGame()
    {
        if (Players.MyPlayer.IsHost && (MyComponents.NetworkManagement.GetNumberPlayers() == EditorVariables.NumberPlayersToStartGame || EditorVariables.NumberPlayersToStartGame == 1))
        {
            Invoke("StartGame", 2f);
            CancelInvoke("CheckStartGame");
        }
    }

    public void StartGame()
    {
        MyComponents.GameState.View.RPC("StartMatch", RPCTargets.Server, true);
    }

    private void SetPlayingState(bool isPlaying)
    {
        this.isPlaying = isPlaying;
        if (Players.MyPlayer.IsHost)
            StartButton.GetComponent<Button>().interactable = !isPlaying;
        CheckTeamButtons();
        Open(!isPlaying);
    }

    public void PutPlayerInTeam(PlayerInfo player, Team team)
    {
        player.transform.SetParent(teamPanels[(int)team].transform, false);
    }

    public void ChangeTeam(int teamNumber)
    {
        Players.MyPlayer.ChangeTeam((Team)teamNumber);
    }

    private void OnDestroy()
    {
        MyComponents.GameState.MatchStartOrEnd -= SetPlayingState;
    }

}