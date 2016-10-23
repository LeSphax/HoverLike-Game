using Byn.Net;
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

    public bool CanPlay
    {
        get
        {
            return MyState != State.STARTING;
        }
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
            state = value;
            if (MyGameObjects.NetworkManagement.isServer)
                View.RPC("SetState", RPCTargets.Others, value);

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
        MyGameObjects.GameInitialization.GameStarted += SendReady;
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
        Players.players[RPCSenderId].isReady = true;
        if (AllPlayersReady())
            StartGameCountdown();

    }

    private void StartGameCountdown()
    {
        Assert.IsTrue(MyState == State.WARMUP);
        MyGameObjects.Countdown.TimerFinished += Entry;
        MyGameObjects.Countdown.TimerFinished += ResetScore;
        MyGameObjects.Countdown.View.RPC("StartTimer", RPCTargets.All, "Warmup", 10f);
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
        Tags.FindPlayers().Map(player => player.GetComponent<PlayerController>().PutAtStartPosition());
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
        Invoke("StartGame", 0.2f - TimeManagement.Latency);
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
