using Byn.Net;
using PlayerManagement;
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
            if (MyGameObjects.NetworkManagement.isServer)
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
            Invoke("Entry", 3f);
            MyState = State.ENDING;
        }
    }

    // Use this for initialization
    void Start()
    {
        MyGameObjects.GameInitialization.AllObjectsCreated += SendReady;
        MyState = State.WARMUP;
    }

    private void SendReady()
    {
        View.RPC("PlayerReady", RPCTargets.Server);
    }

    [MyRPC]
    private void PlayerReady(ConnectionId RPCSenderId)
    {
        Assert.IsTrue(MyState == State.WARMUP);
        Debug.Log("Player Ready " + RPCSenderId);
        Players.players[RPCSenderId].isReady = true;
        if (AllPlayersReady())
            StartGameCountdown();

    }

    private void StartGameCountdown()
    {
        Assert.IsTrue(MyGameObjects.NetworkManagement.isServer && MyState == State.WARMUP);
        MyGameObjects.Countdown.TimerFinished += Entry;
        MyGameObjects.Countdown.TimerFinished += ResetScore;
        MyGameObjects.Countdown.View.RPC("StartTimer", RPCTargets.All, "Warmup", WARMUP_DURATION);
        MyState = State.WARMUP;
    }

    private void ResetScore()
    {
        MyGameObjects.Countdown.TimerFinished -= ResetScore;
        Scoreboard.ResetScore();
    }

    private void Entry()
    {
        Assert.IsTrue(MyGameObjects.NetworkManagement.isServer && (MyState == State.ENDING || MyState == State.WARMUP));
        Tags.FindPlayers().Map(player =>
        {
            PlayerController controller = GetComponent<PlayerController>();
            controller.Player.SpawnNumber = (short)((controller.Player.SpawnNumber + 1) % Players.GetNumberPlayersInTeam(controller.Player.Team));
            controller.View.RPC("ResetPlayer", RPCTargets.All);
        });
        MyGameObjects.BallState.PutAtStartPosition();
        MyGameObjects.Countdown.TimerFinished -= Entry;
        //
        MyGameObjects.Countdown.TimerFinished += SendInvokeStartGame;
        MyGameObjects.Countdown.View.RPC("StartTimer", RPCTargets.All, "Get ready !", 3f);
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

    private bool AllPlayersReady()
    {
        foreach (Player player in Players.players.Values)
        {
            if (!player.isReady)
                return false;
        }
        return true;
    }
}
