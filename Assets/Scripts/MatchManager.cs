using Byn.Net;
using PlayerManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MatchManager : SlideBall.MonoBehaviour
{

    private enum State
    {
        WARMUP,
        STARTING,
        PLAYING,
        ENDING,
    }

    private const float WARMUP_DURATION = 60f;
    private const float END_POINT_DURATION = 5f;
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
            Scoreboard.IncrementTeamScore(teamNumber);
            Invoke("Entry", END_POINT_DURATION);
            MyState = State.ENDING;
        }
    }

    public void StartGameCountdown()
    {
        Assert.IsTrue(MyComponents.NetworkManagement.isServer && MyState == State.WARMUP);
        Entry();
    }

    private void ResetScore()
    {
        MyComponents.Countdown.TimerFinished -= ResetScore;
        Scoreboard.ResetScore();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.M))
        {
            View.RPC("ManualEntry", RPCTargets.Server);
        }
    }

    [MyRPC]
    private void ManualEntry()
    {
        MyState = State.ENDING;
        MyComponents.Countdown.View.RPC("StopTimer", RPCTargets.All);
        Entry();
    }

    private void Entry()
    {
        Assert.IsTrue(MyComponents.NetworkManagement.isServer && (MyState == State.ENDING || MyState == State.WARMUP));
        MyComponents.Countdown.TimerFinished -= Entry;
        StartCoroutine(CoEntry());
    }

    IEnumerator CoEntry()
    {
        PlayerSpawner spawner = gameObject.GetComponent<PlayerSpawner>();
        short syncId = MyComponents.PlayersSynchronisation.GetNewSynchronisationId();
        spawner.View.RPC("DesactivatePlayers", RPCTargets.All, syncId);

        yield return new WaitUntil(() => MyComponents.PlayersSynchronisation.IsSynchronised(syncId));

        SetPlayerRoles();
        MyComponents.PlayersSynchronisation.ResetSyncId(syncId);
        spawner.View.RPC("ResetPlayers", RPCTargets.All, syncId);
        yield return new WaitUntil(() => MyComponents.PlayersSynchronisation.IsSynchronised(syncId));

        MyComponents.PlayersSynchronisation.ResetSyncId(syncId);
        spawner.View.RPC("ReactivatePlayers", RPCTargets.All, syncId);
        yield return new WaitUntil(() => MyComponents.PlayersSynchronisation.IsSynchronised(syncId));

        MyComponents.BallState.PutAtStartPosition();
        //
        if (MyState == State.WARMUP)
        {
            MyComponents.Countdown.TimerFinished += EndWarmup;
            MyComponents.Countdown.TimerFinished += ResetScore;
            MyComponents.Countdown.View.RPC("StartTimer", RPCTargets.All, "Warmup", WARMUP_DURATION);
            MyState = State.WARMUP;
        }
        else if (MyState == State.ENDING)
        {
            MyComponents.Countdown.TimerFinished += SendInvokeStartGame;
            MyComponents.Countdown.View.RPC("StartTimer", RPCTargets.All, "Get ready !", ENTRY_DURATION);
            MyState = State.STARTING;
        }
        else
        {
            Debug.LogError("This shouldn't happen " + MyState);
        }
    }

    private void EndWarmup()
    {
        MyComponents.Countdown.TimerFinished -= EndWarmup;
        MyState = State.ENDING;
        Entry();
    }

    private void SetPlayerRoles()
    {

        foreach (Team team in new Team[2] { Team.FIRST, Team.SECOND })
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

                Player goalie = potentialGoalies[Random.Range(0, potentialGoalies.Count - 1)];
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

    private void SendInvokeStartGame()
    {
        View.RPC("InvokeStartGame", RPCTargets.All);
    }

    [MyRPC]
    private void InvokeStartGame()
    {
        Invoke("StartGame", 0.2f - TimeManagement.LatencyInMiliseconds);
    }

    private void StartGame()
    {
        MyState = State.PLAYING;
    }
}
