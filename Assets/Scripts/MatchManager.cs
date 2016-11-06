using Byn.Net;
using PlayerManagement;
using System.Collections;
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
        MyComponents.Countdown.TimerFinished += Entry;
        MyComponents.Countdown.TimerFinished += ResetScore;
        MyComponents.Countdown.View.RPC("StartTimer", RPCTargets.All, "Warmup", WARMUP_DURATION);
        MyState = State.WARMUP;
    }

    private void ResetScore()
    {
        MyComponents.Countdown.TimerFinished -= ResetScore;
        Scoreboard.ResetScore();
    }



    private void Entry()
    {
        Assert.IsTrue(MyComponents.NetworkManagement.isServer && (MyState == State.ENDING || MyState == State.WARMUP));
        MyComponents.Countdown.TimerFinished -= Entry;
        StartCoroutine(CoEntry());
    }

    IEnumerator CoEntry()
    {
        Debug.Log("Entry");

        PlayerSpawner spawner = gameObject.GetComponent<PlayerSpawner>();
        short syncId = MyComponents.PlayersSynchronisation.GetNewSynchronisationId();
        spawner.View.RPC("DesactivatePlayers", RPCTargets.All, syncId);

        yield return new WaitUntil(() => MyComponents.PlayersSynchronisation.IsSynchronised(syncId));

        Players.players.Values.Map(player =>
        {
            player.SpawnNumber = (short)(((player.SpawnNumber + 1) % Players.GetNumberPlayersInTeam(player.Team)) + 1) ;
            player.AvatarSettingsType = player.SpawnNumber == 0 ? AvatarSettings.AvatarSettingsTypes.GOALIE : AvatarSettings.AvatarSettingsTypes.ATTACKER;
        });
        MyComponents.PlayersSynchronisation.ResetSyncId(syncId);
        spawner.View.RPC("ResetPlayers", RPCTargets.All, syncId);
        yield return new WaitUntil(() => MyComponents.PlayersSynchronisation.IsSynchronised(syncId));

        MyComponents.PlayersSynchronisation.ResetSyncId(syncId);
        spawner.View.RPC("ReactivatePlayers", RPCTargets.All, syncId);
        //Debug.LogWarning("Reactivate");
        yield return new WaitUntil(() => MyComponents.PlayersSynchronisation.IsSynchronised(syncId));

        MyComponents.BallState.PutAtStartPosition();
        //
        MyComponents.Countdown.TimerFinished += SendInvokeStartGame;
        MyComponents.Countdown.View.RPC("StartTimer", RPCTargets.All, "Get ready !", ENTRY_DURATION);
        MyState = State.STARTING;
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
