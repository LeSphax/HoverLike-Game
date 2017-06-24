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
    public static string Password = "";

    private List<GameObject> playerInfos = new List<GameObject>();

    private bool isPlaying;

    public void Open(bool open)
    {
        gameObject.SetActive(open);
        if (open)
        {
            SlideBallInputs.currentPart = SlideBallInputs.GUIPart.MENU;
            RefreshPlayerInfos(ConnectionId.INVALID);
        }
        else
        {
            SlideBallInputs.currentPart = SlideBallInputs.GUIPart.ABILITY;
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
        Debug.LogError("Refresh Players " + id + "    " + Players.players.Count);
        Reset();
        foreach (Player player in Players.players.Values)
        {
            GameObject playerInfo = Instantiate(ResourcesGetter.PlayerInfoPrefab);
            playerInfos.Add(playerInfo);
            playerInfo.GetComponent<PlayerInfo>().TeamChanged += SetPlayerInTeamPanel;
            playerInfo.GetComponent<PlayerInfo>().MyPlayer = player;
        }
    }

    private void SetPlayerInTeamPanel(PlayerInfo playerInfo, Team team)
    {
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

    private void Start()
    {
        isPlaying = false;
        MyComponents.GameInitialization.MatchStartOrEnd += SetPlayingState;

        if (EditorVariables.StartGameImmediately)
        {
            InvokeRepeating("CheckStartGame", 0f, 0.2f);
        }
        Players.NewPlayerCreated += RefreshPlayerInfos;
    }

    void CheckStartGame()
    {
        if (MyComponents.NetworkManagement.IsServer && (MyComponents.NetworkManagement.GetNumberPlayers() == EditorVariables.NumberPlayersToStartGame || EditorVariables.NumberPlayersToStartGame == 1))
        {
            Invoke("StartGame", 2f);
            CancelInvoke("CheckStartGame");
        }
    }

    public void StartGame()
    {
        MyComponents.GameInitialization.StartMatch(true);
    }

    private void SetPlayingState(bool isPlaying)
    {
        this.isPlaying = isPlaying;
        if (MyComponents.NetworkManagement.IsServer)
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
        MyComponents.GameInitialization.MatchStartOrEnd -= SetPlayingState;
    }

}