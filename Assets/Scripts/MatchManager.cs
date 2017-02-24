﻿using Byn.Net;
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

    private Team lastChanceTeam = Team.NONE;

    public enum State : byte
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
        matchCountdown.PauseTimer(true);
        Scoreboard.IncrementTeamScore(teamNumber);
        Invoke(methodName, END_POINT_DURATION);
        MyState = State.ENDING;
        if (lastChanceTeam != Team.NONE)
            EndMatch();
    }

    public void StartGame()
    {
        Assert.IsTrue(MyComponents.NetworkManagement.isServer && MyState == State.WARMUP);
        Players.PlayerOwningBallChanged += BallChangedOwner;
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
        matchCountdown.TimerFinished += LastChance;
        matchCountdown.View.RPC("StartMatch", RPCTargets.All, MATCH_DURATION, 10);
        MyState = State.ENDING;
        Scoreboard.ResetScore();
        Entry();
    }

    private void LastChance()
    {
        if (MyComponents.BallState.GetIdOfPlayerOwningBall() == BallState.NO_PLAYER_ID)
        {
            matchCountdown.PlayMatchEndSound();
            EndMatch();
        }
        else
        {
            lastChanceTeam = Players.players[MyComponents.BallState.GetIdOfPlayerOwningBall()].Team;
        }
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
        StartCoroutine(CoEntry());
    }

    IEnumerator CoEntry()
    {
        Debug.Log("MatchManager : CoEntry - State : " + MyState);
        Assert.IsTrue(MyState == State.ENDING || MyState == State.WARMUP || MyState == State.SUDDEN_DEATH, "The entry shouldn't happen in this case, maybe it was a manual entry?");
        if (MyState == State.ENDING)
        {
            getReadyCountdown.TimerFinished += SendInvokeStartRound;
            getReadyCountdown.View.RPC("StartTimer", RPCTargets.All, ENTRY_DURATION, "Go !");
            MyState = State.STARTING;
        }
        PlayerSpawner spawner = gameObject.GetComponent<PlayerSpawner>();
        short syncId = MyComponents.PlayersSynchronisation.GetNewSynchronisationId();
        spawner.View.RPC("DesactivatePlayers", RPCTargets.All, syncId);

        yield return new WaitUntil(() => MyComponents.PlayersSynchronisation.IsSynchronised(syncId));

        MyComponents.BallState.PutAtStartPosition();
        SetPlayerRoles();
        MyComponents.Players.SendChanges();
        MyComponents.PlayersSynchronisation.ResetSyncId(syncId);
        spawner.View.RPC("ResetPlayers", RPCTargets.All, syncId);
        yield return new WaitUntil(() => MyComponents.PlayersSynchronisation.IsSynchronised(syncId));

        MyComponents.PlayersSynchronisation.ResetSyncId(syncId);
        spawner.View.RPC("ReactivatePlayers", RPCTargets.All, syncId);
        yield return new WaitUntil(() => MyComponents.PlayersSynchronisation.IsSynchronised(syncId));

        //
        if (MyState == State.WARMUP)
            View.RPC("ShowGame", RPCTargets.All);
    }

    [MyRPC]
    private static void ShowGame()
    {
        Debug.Log("MatchManager : ShowGame");
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

                //The spawns have never been assigned => all players have 0 as a spawnNumber
                if (players[0].SpawnNumber == 0)
                {
                    short i = 1;
                    players.Map(player => { player.SpawnNumber = i; i++; });
                }
                players.Map(player =>
                {
                    Debug.Log("Before " + player.SpawnNumber);
                    int attackersNumber = Players.GetPlayersInTeam(player.Team).Count - 1;
                    player.SpawnNumber = (short)(((player.SpawnNumber) % attackersNumber) + 1);
                    Debug.Log("After " + player.SpawnNumber);
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

    [MyRPC]
    public void ManualEnd()
    {
        matchCountdown.TimeLeft = 2;
    }

    [MyRPC]
    public void ManualScoreGoal(int teamNumber)
    {
        TeamScored(teamNumber);
    }

    [MyRPC]
    public void ManualEntry()
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

    public void BallChangedOwner(ConnectionId previousPlayer, ConnectionId newPlayer)
    {
        switch (MyState)
        {
            case State.PLAYING:
                matchCountdown.PauseTimer(false);
                if (lastChanceTeam != Team.NONE)
                {
                    if (newPlayer != BallState.NO_PLAYER_ID &&  Players.players[newPlayer].Team != lastChanceTeam)
                    {
                        matchCountdown.PlayMatchEndSound();
                        Invoke("EndMatch", END_POINT_DURATION);
                    }
                }
                break;
            default:
                //Do nothing
                break;
        }
    }
}
