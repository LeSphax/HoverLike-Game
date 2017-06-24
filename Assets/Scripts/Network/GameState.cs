using Navigation;
using PlayerManagement;
using UnityEngine;

public delegate void MatchChange(bool started);

public class GameState : SlideBall.MonoBehaviour
{
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

    protected void Awake()
    {
        Reset();
    }

    private void SetupScene()
    {
        if (Scenes.MainIndex == Scenes.currentSceneId)
        {
            MyComponents.WarmupManager.Activate(true);

            if (MyComponents.NetworkManagement.IsServer)
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
        if (MyComponents.NetworkManagement.IsServer)
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

        foreach (var player in Players.players.Values)
        {
            player.gameobjectAvatar = Functions.InstantiatePlayer(player.id);
        }
    }

    public void InitGame()
    {
        if (MyComponents.NetworkManagement.IsServer)
            LoadRoom();
        else
            Players.PlayersDataReceived += LoadRoom;
    }

    private static void LoadRoom()
    {
        NavigationManager.LoadScene(Scenes.Main, false, false);
        Players.PlayersDataReceived -= LoadRoom;
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
