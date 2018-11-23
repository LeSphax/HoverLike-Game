using Ball;
using Byn.Net;
using Navigation;
using PlayerManagement;
using UnityEngine;

public delegate void MatchChange(bool started);

public class GameState : SlideBall.NetworkMonoBehaviour, IGameInit
{

    public const float playerOriginalYPosition = 4.4f;
    bool started;

    public event EmptyEventHandler AllObjectsCreated;
    public event MatchChange MatchStartOrEnd;

    public void AddGameStartedListener(EmptyEventHandler handler)
    {
        if (started)
            handler.Invoke();
        else
            AllObjectsCreated += handler;
    }

    protected void Start()
    {
        Reset();
    }

    private void SetupScene()
    {
        if (Scenes.MainIndex == Scenes.currentSceneId)
        {
            MyComponents.WarmupManager.Activate(true);

            if (NetworkingState.IsServer)
            {
                InstantiateNewObjects();
                MyComponents.MatchManager.Activate(false);
            }
            ResourcesGetter.LoadAll();
            NavigationManager.ShowLevel();
            MyComponents.VictoryPose.PlayAgain += PlayAgain;
        }
    }

    [MyRPC]
    public void StartMatch(bool start)
    {
        if (NetworkingState.IsServer)
        {
            View.RPC("StartMatch", RPCTargets.Others, start);
            MyComponents.MatchManager.Activate(start);
            MyComponents.NetworkManagement.CurrentlyPlaying = start;
        }
        MyComponents.WarmupManager.Activate(!start);
        if (MatchStartOrEnd != null)
        {
            MatchStartOrEnd.Invoke(start);
        }
    }

    private void InstantiateNewObjects()
    {
        MyComponents.NetworkViewsManagement.Instantiate("Ball", MyComponents.Spawns.BallSpawn, Quaternion.identity);

        foreach (var player in MyComponents.Players.players.Values)
        {
            InstantiatePlayer(MyComponents.NetworkViewsManagement, player.id);
        }
    }

    public static GameObject InstantiatePlayer(NetworkViewsManagement viewsManagement, ConnectionId id)
    {
        return viewsManagement.Instantiate("MyPlayer", new Vector3(0, playerOriginalYPosition, 0), Quaternion.identity, id);
    }

    public void InitGame()
    {
        if (NetworkingState.IsServer)
            LoadRoom();
        else
            MyComponents.Players.PlayersDataReceived += LoadRoom;
    }

    private void LoadRoom()
    {
        NavigationManager.LoadScene(Scenes.Main, false, false);
        MyComponents.Players.PlayersDataReceived -= LoadRoom;
    }

    public void Reset()
    {
        started = false;
        MyComponents.NetworkManagement.ReceivedAllBufferedMessages -= SetupScene;
        MyComponents.NetworkManagement.ReceivedAllBufferedMessages += SetupScene;


        AllObjectsCreated = null;
    }

    private void PlayAgain()
    {
        StartMatch(false);
    }
}
