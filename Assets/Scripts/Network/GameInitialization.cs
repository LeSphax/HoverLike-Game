using Byn.Net;
using Navigation;
using PlayerManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class GameInitialization : SlideBall.MonoBehaviour
{
    bool started;
    short syncId;

    public event EmptyEventHandler AllObjectsCreated;

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

    public void StartGame()
    {
        Debug.Log("StartGame");
        Assert.IsTrue(MyComponents.NetworkManagement.IsServer);

        // short syncId = MyComponents.PlayersSynchronisation.GetNewSynchronisationId();
        Debug.Log("Before LoadRoom");
        View.RPC("LoadRoom", RPCTargets.All);
    }

    private void SetupScene(ConnectionId id, short newSceneId)
    {
        InstantiateNewObjects();
        MyComponents.MatchManager.StartGame();
    }

    [MyRPC]
    private void LoadRoom()
    {
        Debug.Log("LoadRoom");
        if (MyComponents.NetworkManagement.IsServer) {

            NavigationManager.LoadScene(Scenes.Main, true, true);
            Players.MyPlayer.SceneChanged += SetupScene;
        }
        else
        {
            NavigationManager.LoadScene(Scenes.Main, true, true);
        }
        Debug.Log("EndLoadRoom");
    }

    IEnumerator WaitThenLoad()
    {
        float t = Random.Range(0, 10);
        if (((int)t) % 2 == 0)
            t = 0;
        yield return new WaitForSeconds(10);
        NavigationManager.LoadScene(Scenes.Main, true, true);
    }

    private void InstantiateNewObjects()
    {
        MyComponents.NetworkViewsManagement.Instantiate("Ball", MyComponents.Spawns.BallSpawn, Quaternion.identity);

        foreach (var id in Players.players.Keys)
        {
            MyComponents.NetworkViewsManagement.Instantiate("MyPlayer", new Vector3(0, 4.4f, 0), Quaternion.identity, id);
        }
    }

    private void SendReady(ConnectionId id, short newSceneId)
    {
        Debug.Log("SendReady");
        SendReady(this.syncId);
    }

    [MyRPC]
    private void SendReady(short syncId)
    {
        MyComponents.PlayersSynchronisation.SendSynchronisation(syncId);
    }

    public void Reset()
    {
        started = false;
        syncId = PlayersSynchronisation.INVALID_SYNC_ID;
        AllObjectsCreated = null;
        if (Players.MyPlayer != null)
            Players.MyPlayer.SceneChanged -= SendReady;
    }
}
