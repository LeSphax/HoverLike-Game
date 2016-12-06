using Byn.Net;
using Navigation;
using PlayerManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MatchManager : SlideBall.MonoBehaviour
{
    [SerializeField]
    private WarmupCountdown matchCountdown;
    [SerializeField]
    private Countdown getReadyCountdown;

    private enum State
    {
        WARMUP,
        STARTING,
        PLAYING,
        ENDING,
        SUDDEN_DEATH,
    }

    private const float END_POINT_DURATION = 3f;
    private const float MATCH_DURATION = 300f;
    private const float ENTRY_DURATION = 3f;
    private State state;
    private State MyState
    {
        get
        {
            return state;
        }
        set
        {
            state = value;
            if (MyComponents.NetworkManagement.isServer)
            {
                View.RPC("SetState", RPCTargets.Others, value);
                switch (state)
                {
                    case State.WARMUP:
                    case State.PLAYING:
                    case State.SUDDEN_DEATH:
                    case State.ENDING:
                        Players.SetState(Player.State.PLAYING);
                        break;
                    case State.STARTING:
                        Players.SetState(Player.State.FROZEN);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    [MyRPC]
    private void SetState(State newState)
    {
        MyState = newState;
    }

    internal void TeamScored(int teamNumber)
    {
        if (MyState == State.PLAYING)
        {
            EndRound(teamNumber, "Entry");
        }
        else if (MyState == State.SUDDEN_DEATH)
        {
            EndRound(teamNumber, "EndMatch");
        }
    }

    private void EndRound(int teamNumber, string methodName)
    {
        Scoreboard.IncrementTeamScore(teamNumber);
        Invoke(methodName, END_POINT_DURATION);
        MyState = State.ENDING;
    }

    public void StartGame()
    {
        Assert.IsTrue(MyComponents.NetworkManagement.isServer && MyState == State.WARMUP);
        StartCoroutine(Warmup());
        Entry();
    }

    IEnumerator Warmup()
    {
        short syncId = MyComponents.PlayersSynchronisation.GetNewSynchronisationId();
        matchCountdown.View.RPC("StartWarmup", RPCTargets.All, syncId);
        MyState = State.WARMUP;
        //Wait until everyone press the ready button
        yield return new WaitUntil(() => MyComponents.PlayersSynchronisation.IsSynchronised(syncId));
        matchCountdown.TimerFinished += EndMatch;
        matchCountdown.View.RPC("StartTimer", RPCTargets.All, MATCH_DURATION);
        MyState = State.ENDING;
        Scoreboard.ResetScore();
        Entry();
    }

    [MyRPC]
    private void SuddenDeath()
    {
        Debug.LogError("SuddenDeath");
        matchCountdown.SetText(Language.Instance.texts["Sudden_Death"]);
    }

    private void EndMatch()
    {
        Debug.LogError("EndMatch " + Scoreboard.GetWinningTeam());
        Team winningTeam = Scoreboard.GetWinningTeam();
        if (winningTeam == Team.NONE)
        {
            View.RPC("SuddenDeath", RPCTargets.All);
            MyState = State.SUDDEN_DEATH;
            Entry();
        }
        else
        {
            CancelInvoke("Entry");
            getReadyCountdown.View.RPC("StopTimer", RPCTargets.All);
            matchCountdown.View.RPC("StopTimer", RPCTargets.All);
            View.RPC("SetVictoryPose", RPCTargets.All, winningTeam);
        }
    }

    [MyRPC]
    private void SetVictoryPose(Team team)
    {
        MyComponents.VictoryPose.SetVictoryPose(team);
    }

    private void Entry()
    {
        Assert.IsTrue(MyComponents.NetworkManagement.isServer && (MyState == State.ENDING || MyState == State.WARMUP || MyState == State.SUDDEN_DEATH));
        Debug.Log("Entry " + MyState);
        StartCoroutine(CoEntry());
    }

    IEnumerator CoEntry()
    {
        Assert.IsTrue(MyState == State.ENDING || MyState == State.WARMUP || MyState == State.SUDDEN_DEATH);
        if (MyState == State.ENDING)
        {
            getReadyCountdown.TimerFinished += SendInvokeStartRound;
            getReadyCountdown.View.RPC("StartTimer", RPCTargets.All, ENTRY_DURATION);
            MyState = State.STARTING;
        }
        PlayerSpawner spawner = gameObject.GetComponent<PlayerSpawner>();
        short syncId = MyComponents.PlayersSynchronisation.GetNewSynchronisationId();
        spawner.View.RPC("DesactivatePlayers", RPCTargets.All, syncId);

        yield return new WaitUntil(() => MyComponents.PlayersSynchronisation.IsSynchronised(syncId));

        SetPlayerRoles();
        MyComponents.Players.SendChanges();
        MyComponents.PlayersSynchronisation.ResetSyncId(syncId);
        spawner.View.RPC("ResetPlayers", RPCTargets.All, syncId);
        yield return new WaitUntil(() => MyComponents.PlayersSynchronisation.IsSynchronised(syncId));

        MyComponents.PlayersSynchronisation.ResetSyncId(syncId);
        spawner.View.RPC("ReactivatePlayers", RPCTargets.All, syncId);
        yield return new WaitUntil(() => MyComponents.PlayersSynchronisation.IsSynchronised(syncId));

        MyComponents.BallState.PutAtStartPosition();
        //
        if (MyState == State.WARMUP)
            View.RPC("ShowGame", RPCTargets.All);
    }

    [MyRPC]
    private static void ShowGame()
    {
        NavigationManager.ShowLevel();
    }

    private void SetPlayerRoles()
    {

        foreach (Team team in new Team[2] { Team.BLUE, Team.RED })
        {
            List<Player> players = Players.GetPlayersInTeam(team);
            if (players.Count == 1)
            {
                players[0].SpawnNumber = players[0].PlayAsGoalie ? (short)0 : (short)1;
                players[0].AvatarSettingsType = players[0].SpawnNumber == 0 ? AvatarSettings.AvatarSettingsTypes.GOALIE : AvatarSettings.AvatarSettingsTypes.ATTACKER;
            }
            else if (players.Count > 1)
            {
                Player oldGoalie = null;
                players.Map(player => { if (player.AvatarSettingsType == AvatarSettings.AvatarSettingsTypes.GOALIE) oldGoalie = player; });
                Assert.IsTrue(oldGoalie != null);

                List<Player> potentialGoalies = new List<Player>();
                players.Map(player => { if (player.PlayAsGoalie) potentialGoalies.Add(player); });
                if (potentialGoalies.Count == 0)
                    potentialGoalies = new List<Player>(players);

                Player goalie = potentialGoalies[Random.Range(0, potentialGoalies.Count)];
                oldGoalie.SpawnNumber = goalie.SpawnNumber;
                goalie.SpawnNumber = 0;
                goalie.AvatarSettingsType = AvatarSettings.AvatarSettingsTypes.GOALIE;
                players.Remove(goalie);

                players.Map(player =>
                {
                    player.SpawnNumber = (short)(((player.SpawnNumber + 1) % (Players.GetPlayersInTeam(player.Team).Count - 1)) + 1);
                    player.AvatarSettingsType = AvatarSettings.AvatarSettingsTypes.ATTACKER;
                });
            }
        }
    }

    private void SendInvokeStartRound()
    {
        getReadyCountdown.TimerFinished -= SendInvokeStartRound;
        View.RPC("InvokeStartRound", RPCTargets.All);
    }

    [MyRPC]
    private void InvokeStartRound()
    {
        Invoke("StartRound", 0.2f - TimeManagement.LatencyInMiliseconds);
    }

    private void StartRound()
    {
        MyState = State.PLAYING;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetKeyDown(KeyCode.S))
                View.RPC("SetReady", RPCTargets.All);
            else if (Input.GetKeyDown(KeyCode.E))
                View.RPC("ManualEnd", RPCTargets.All);
            else if (Input.GetKeyDown(KeyCode.M))
                View.RPC("ManualEntry", RPCTargets.Server);
            else if (Input.GetKeyDown(KeyCode.B))
                View.RPC("ManualScoreGoal", RPCTargets.Server, 0);
            else if (Input.GetKeyDown(KeyCode.R))
                View.RPC("ManualScoreGoal", RPCTargets.Server, 1);
        }
    }

    [MyRPC]
    private void ManualEnd()
    {
        matchCountdown.SetTimeLeft(2);
    }

    [MyRPC]
    private void ManualScoreGoal(int teamNumber)
    {
        TeamScored(teamNumber);
    }

    [MyRPC]
    private void ManualEntry()
    {
        MyState = State.ENDING;
        getReadyCountdown.View.RPC("StopTimer", RPCTargets.All);
        Entry();
    }

    [MyRPC]
    private void SetReady()
    {
        matchCountdown.StopWarmup();
    }
}
