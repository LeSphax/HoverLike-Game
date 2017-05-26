using AbilitiesManagement;
using Ball;
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
    private MatchCountdown matchCountdown;
    [SerializeField]
    private Countdown entryCountdown;

    private Team lastChanceTeam = Team.NONE;

    private bool suddenDeath = false;

    public enum State : byte
    {
        LOADING_SCENE,
        WARMUP,
        STARTING,
        BEFORE_BALL_PICKED,
        PLAYING,
        ENDING_ROUND,
        LAST_CHANCE,
        SUDDEN_DEATH,
        VICTORY_POSE,
    }

    private const float END_POINT_DURATION = 3f;
    private const float MATCH_DURATION = 300f;
    private const float WARMUP_DURATION = 90f;
    private const float MATCH_END_DURATION = 10f;
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
            Debug.Log("Set State " + value);
            State previousState = state;
            state = value;
            if (MyComponents.NetworkManagement.isServer)
            {
                View.RPC("SetState", RPCTargets.Others, value);
                switch (MyState)
                {
                    case State.LOADING_SCENE:
                        break;
                    case State.WARMUP:
                        StartCoroutine(Warmup());
                        break;
                    case State.STARTING:
                        break;
                    case State.BEFORE_BALL_PICKED:
                        break;
                    case State.PLAYING:
                        break;
                    case State.ENDING_ROUND:
                        EmptyEventHandler method;

                        switch (previousState)
                        {
                            case State.WARMUP:
                                method = Entry;
                                break;
                            case State.PLAYING:
                                if (matchCountdown.TimeLeft <= 0)
                                {
                                    method = EndMatch;
                                }
                                else
                                    method = Entry;
                                break;
                            case State.LAST_CHANCE:
                            case State.SUDDEN_DEATH:
                                entryCountdown.PlayMatchEndSound();
                                method = EndMatch;
                                break;
                            default:
                                Debug.LogError("It shouldn't be possible to get to " + State.ENDING_ROUND + " from " + previousState);
                                return;
                        }
                        Invoke(method.Method.Name, END_POINT_DURATION);
                        break;
                    case State.LAST_CHANCE:
                        if (MyComponents.BallState.GetIdOfPlayerOwningBall() == BallState.NO_PLAYER_ID)
                        {
                            EndMatch();
                        }
                        else
                        {
                            lastChanceTeam = Players.players[MyComponents.BallState.GetIdOfPlayerOwningBall()].Team;
                        }
                        break;
                    case State.SUDDEN_DEATH:
                        break;
                    case State.VICTORY_POSE:
                        CancelInvoke("Entry");
                        break;
                    default:
                        break;
                }
                switch (MyState)
                {
                    case State.LOADING_SCENE:
                    case State.WARMUP:
                    case State.BEFORE_BALL_PICKED:
                    case State.PLAYING:
                    case State.LAST_CHANCE:
                    case State.SUDDEN_DEATH:
                        Players.SetState(Player.State.PLAYING);
                        break;
                    case State.STARTING:
                    case State.ENDING_ROUND:
                    case State.VICTORY_POSE:
                        Players.SetState(Player.State.FROZEN);
                        break;
                    default:
                        break;
                }
            }
            switch (MyState)
            {
                case State.LOADING_SCENE:
                    break;
                case State.WARMUP:
                    break;
                case State.PLAYING:
                    break;
                case State.LAST_CHANCE:
                    matchCountdown.StopTimerAndSetText(Language.Instance.texts["Last_Chance"]);
                    break;
                case State.ENDING_ROUND:
                case State.STARTING:
                case State.BEFORE_BALL_PICKED:
                case State.SUDDEN_DEATH:
                    if (suddenDeath)
                        matchCountdown.StopTimerAndSetText(Language.Instance.texts["Sudden_Death"]);
                    break;
                case State.VICTORY_POSE:
                    matchCountdown.StopTimerAndSetText("");
                    entryCountdown.StopTimerAndSetText("");
                    break;
                default:
                    break;
            }
            switch (MyState)
            {
                case State.LOADING_SCENE:
                case State.BEFORE_BALL_PICKED:
                case State.LAST_CHANCE:
                case State.STARTING:
                case State.ENDING_ROUND:
                case State.SUDDEN_DEATH:
                case State.VICTORY_POSE:
                    matchCountdown.PauseTimer(true);
                    break;
                case State.WARMUP:
                case State.PLAYING:
                    matchCountdown.PauseTimer(false);
                    break;
                default:
                    break;
            }
            switch (MyState)
            {
                case State.LOADING_SCENE:
                case State.WARMUP:
                case State.LAST_CHANCE:
                case State.ENDING_ROUND:
                case State.SUDDEN_DEATH:
                case State.VICTORY_POSE:
                    entryCountdown.StopTimerAndSetText("");
                    break;
                case State.STARTING:
                    entryCountdown.PauseTimer(false);
                    break;
                case State.BEFORE_BALL_PICKED:
                    entryCountdown.StopTimerAndSetText("Go !");
                    break;
                case State.PLAYING:
                    if (matchCountdown.TimeLeft <= MATCH_END_DURATION)
                    {
                        StartLastSecondsTimer();
                    }
                    else
                    {
                        entryCountdown.StopTimerAndSetText("");
                        matchCountdown.RegisterCloseToEnd(StartLastSecondsTimer, MATCH_END_DURATION);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private void StartLastSecondsTimer()
    {
        entryCountdown.StartTimer(matchCountdown.TimeLeft);
        entryCountdown.PauseTimer(false);
        matchCountdown.RegisterCloseToEnd(StartLastSecondsTimer, MATCH_END_DURATION, false);
    }

    [MyRPC]
    private void SetState(State newState)
    {
        MyState = newState;
    }

    public void StartGame()
    {
        Assert.IsTrue(MyComponents.NetworkManagement.isServer);
        Players.PlayerOwningBallChanged += BallChangedOwner;
        matchCountdown.TimerFinished += MatchCountdownTimerFinished;
        entryCountdown.TimerFinished += EntryCountdownTimerFinished;
        MyState = State.LOADING_SCENE;
        Entry();
    }

    IEnumerator Warmup()
    {
        short syncId = MyComponents.PlayersSynchronisation.GetNewSynchronisationId();
        matchCountdown.View.RPC("StartWarmup", RPCTargets.All, WARMUP_DURATION, syncId);
        Entry();
        //Wait until everyone press the ready button
        yield return new WaitUntil(() => MyComponents.PlayersSynchronisation.IsSynchronised(syncId));
        entryCountdown.PlayMatchEndSound();
        matchCountdown.View.RPC("StartMatch", RPCTargets.All, MATCH_DURATION, 10);
        Scoreboard.ResetScore();
        MyState = State.ENDING_ROUND;
    }

    private void EndMatch()
    {
        Debug.Log("EndMatch " + Scoreboard.GetWinningTeam());
        //Don't play the gong sound at the same time as the "But" sound

        Team winningTeam = Scoreboard.GetWinningTeam();
        if (winningTeam == Team.NONE)
        {
            suddenDeath = true;
            Entry();
        }
        else
        {
            SetVictoryPose();
        }
    }

    private void SetVictoryPose()
    {
        Team winningTeam = Scoreboard.GetWinningTeam();
        MyState = State.VICTORY_POSE;
        View.RPC("SetVictoryPose", RPCTargets.All, winningTeam);
    }

    [MyRPC]
    private void SetVictoryPose(Team team)
    {
        MyComponents.VictoryPose.SetVictoryPose(team);
    }



    private void FinishedUsingAllAbilities()
    {
        Debug.Log("FinishedUsingAllAbilities");
        switch (MyState)
        {
            case State.LOADING_SCENE:
                if (Players.MyPlayer.AvatarSettingsType == AvatarSettings.AvatarSettingsTypes.ATTACKER)
                {
                    Players.MyPlayer.PlayAsGoalie = true;
                    Entry();
                }
                else
                {
                    Debug.Log("FinishedUsingAllAbilities Start Warmup");
                    Players.MyPlayer.PlayAsGoalie = EditorVariables.PlayAsGoalieInitialValue;
                    Players.MyPlayer.gameobjectAvatar.GetComponent<AIRandomMovement>().FinishedUsingAllAbilities -= FinishedUsingAllAbilities;
                    MyState = State.WARMUP;
                }
                break;
            default:
                break;
        }
    }

    [MyRPC]
    private void ShowGame()
    {
        Debug.Log("MatchManager : ShowGame");
        ResourcesGetter.LoadAll();
        NavigationManager.ShowLevel();
    }

    #region Entry

    private void Entry()
    {
        StartCoroutine(CoEntry());
    }

    IEnumerator CoEntry()
    {
        Debug.Log("MatchManager : CoEntry - State : " + MyState);
        Assert.IsTrue(MyState == State.ENDING_ROUND || MyState == State.WARMUP || MyState == State.SUDDEN_DEATH || MyState == State.LOADING_SCENE, "The entry shouldn't happen in this state " + MyState + " , maybe it was a manual entry?");

        switch (MyState)
        {
            case State.LOADING_SCENE:
                yield return EntryPlayersCreation();
                if (Players.MyPlayer.AvatarSettingsType == AvatarSettings.AvatarSettingsTypes.ATTACKER)
                {
                    Players.MyPlayer.gameobjectAvatar.GetComponent<AIRandomMovement>().FinishedUsingAllAbilities += FinishedUsingAllAbilities;
                    Players.MyPlayer.gameobjectAvatar.GetComponent<AIRandomMovement>().UseAllAbilities(true);
                }
                else if (Players.MyPlayer.AvatarSettingsType == AvatarSettings.AvatarSettingsTypes.GOALIE)
                {
                    Players.MyPlayer.gameobjectAvatar.GetComponent<AIRandomMovement>().UseAllAbilities(false);
                }
                break;
            case State.WARMUP:
                yield return EntryPlayersCreation();
                View.RPC("ShowGame", RPCTargets.All);
                break;
            case State.ENDING_ROUND:
                entryCountdown.View.RPC("StartTimer", RPCTargets.All, ENTRY_DURATION);
                MyState = State.STARTING;
                yield return EntryPlayersCreation();
                break;
            case State.LAST_CHANCE:
            case State.STARTING:
            case State.PLAYING:
            case State.VICTORY_POSE:
                Debug.LogError("The entry shouldn't happen in this state " + MyState);
                break;
            default:
                break;
        }
    }

    IEnumerator EntryPlayersCreation()
    {
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
                players.Map(player =>
                {
                    if (player.AvatarSettingsType == AvatarSettings.AvatarSettingsTypes.GOALIE
|| player.AvatarSettingsType == AvatarSettings.AvatarSettingsTypes.NONE) oldGoalie = player;
                });
                Assert.IsTrue(oldGoalie != null);

                List<Player> potentialGoalies = new List<Player>();
                if (MyState == State.LOADING_SCENE && team == Players.MyPlayer.Team)
                {
                    if (Players.MyPlayer.PlayAsGoalie)
                        potentialGoalies.Add(Players.MyPlayer);
                    else
                    {
                        potentialGoalies = new List<Player>(players);
                        potentialGoalies.Remove(Players.MyPlayer);
                    }
                }
                else
                {
                    players.Map(player => { if (player.PlayAsGoalie) potentialGoalies.Add(player); });
                    if (potentialGoalies.Count == 0)
                        potentialGoalies = new List<Player>(players);
                }

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
                    int attackersNumber = Players.GetPlayersInTeam(player.Team).Count - 1;
                    player.SpawnNumber = (short)(((player.SpawnNumber) % attackersNumber) + 1);
                    player.AvatarSettingsType = AvatarSettings.AvatarSettingsTypes.ATTACKER;
                });

            }
        }

        AllowAttackersToEnterGoalsWhenAlone();
    }

    private void AllowAttackersToEnterGoalsWhenAlone()
    {
        Physics.IgnoreLayerCollision(LayersGetter.ATTACKER_0, LayersGetter.GOAL_0, Players.GetPlayersInTeam(Team.BLUE).Count == 1);
        Physics.IgnoreLayerCollision(LayersGetter.ATTACKER_1, LayersGetter.GOAL_1, Players.GetPlayersInTeam(Team.RED).Count == 1);
    }

    private void SendInvokeStartRound()
    {
        View.RPC("InvokeStartRound", RPCTargets.All);
    }

    [MyRPC]
    private void InvokeStartRound()
    {
        Invoke("StartRound", 0.2f - TimeManagement.LatencyInMiliseconds);
    }

    private void StartRound()
    {
        MyState = State.BEFORE_BALL_PICKED;
    }
    #endregion

    #region EventHandlers

    public void EntryCountdownTimerFinished()
    {
        switch (MyState)
        {
            case State.STARTING:
                SendInvokeStartRound();
                MyState = State.BEFORE_BALL_PICKED;
                break;
            case State.PLAYING:
                //End of match
                break;
            default:
                Debug.LogError("The entry timer shouldn't end in this state " + MyState);
                break;
        }
    }

    public void MatchCountdownTimerFinished()
    {
        Debug.Log("Timer Finished " + MyState);

        switch (MyState)
        {
            case State.PLAYING:
                if (MyComponents.BallState.IdPlayerOwningBall != BallState.NO_PLAYER_ID)
                {
                    MyState = State.LAST_CHANCE;
                }
                else
                {
                    MyState = State.ENDING_ROUND;
                }
                break;
            case State.WARMUP:
                MyComponents.MatchManager.View.RPC("SetReady", RPCTargets.All);
                break;
            default:
                Debug.LogError("The match timer shouldn't end in this state " + MyState);
                break;
        }
    }

    internal void TeamScored(int teamNumber)
    {
        switch (MyState)
        {
            case State.LOADING_SCENE:
                break;
            case State.WARMUP:
                break;
            case State.STARTING:
                break;
            case State.BEFORE_BALL_PICKED:
                break;
            case State.ENDING_ROUND:
                break;
            case State.PLAYING:
            case State.LAST_CHANCE:
            case State.SUDDEN_DEATH:
                if (Scoreboard.IncrementTeamScore(teamNumber))
                {
                    MyState = State.ENDING_ROUND;
                }
                break;
            case State.VICTORY_POSE:
                break;
            default:
                break;
        }

    }

    public void BallChangedOwner(ConnectionId previousPlayer, ConnectionId newPlayer)
    {
        //Debug.Log("BallChangedOwner " + MyState + "    " + lastChanceTeam);
        switch (MyState)
        {
            case State.BEFORE_BALL_PICKED:
                MyState = suddenDeath ? State.SUDDEN_DEATH : State.PLAYING;
                break;
            case State.LAST_CHANCE:
                if (newPlayer != BallState.NO_PLAYER_ID && Players.players[newPlayer].Team != lastChanceTeam)
                {
                    Debug.Log("BallChangedOwner " + lastChanceTeam + "    " + Players.players[newPlayer].Team);
                    MyState = State.ENDING_ROUND;
                }
                break;
            default:
                //Do nothing
                //Debug.LogError("The player shouldn't be able to pick up the ball in this state " + MyState);
                break;
        }
    }

    private void OnDestroy()
    {
        Players.PlayerOwningBallChanged -= BallChangedOwner;
        matchCountdown.TimerFinished -= MatchCountdownTimerFinished;
        entryCountdown.TimerFinished -= EntryCountdownTimerFinished;
    }
    #endregion

    #region DevCommands
    [MyRPC]
    public void ManualEnd()
    {
        if (matchCountdown.TimeLeft < 20)
            matchCountdown.TimeLeft = 2;
        else
            matchCountdown.TimeLeft = 12;


    }

    [MyRPC]
    public void ManualScoreGoal(int teamNumber)
    {
        TeamScored(teamNumber);
    }

    [MyRPC]
    public void ManualEntry()
    {
        switch (MyState)
        {
            case State.LOADING_SCENE:
                MyState = State.LOADING_SCENE;
                break;
            case State.WARMUP:
                MyState = State.WARMUP;
                break;
            case State.STARTING:
            case State.BEFORE_BALL_PICKED:
            case State.PLAYING:
            case State.ENDING_ROUND:
            case State.LAST_CHANCE:
            case State.VICTORY_POSE:
                MyState = State.PLAYING;
                break;
            case State.SUDDEN_DEATH:
                MyState = State.SUDDEN_DEATH;
                break;
            default:
                break;
        }
        MyState = State.ENDING_ROUND;
    }

    [MyRPC]
    private void SetReady()
    {
        matchCountdown.StopWarmup();
    }
    #endregion
}
