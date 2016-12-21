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
        StartCoroutine(CoStartGame());
    }

    public IEnumerator CoStartGame()
    {
        Assert.IsTrue(MyComponents.NetworkManagement.isServer);

        short syncId = MyComponents.PlayersSynchronisation.GetNewSynchronisationId();
        Debug.Log("Before LoadRoom");
        View.RPC("LoadRoom", RPCTargets.All, syncId);
        yield return new WaitUntil(() => MyComponents.PlayersSynchronisation.IsSynchronised(syncId));
        //
        Debug.Log("InstantiateObjects");
        syncId = MyComponents.PlayersSynchronisation.GetNewSynchronisationId();
        InstantiateNewObjects();
        Debug.Log("AfterInstantiate");
        View.RPC("SendReady", RPCTargets.All, syncId);
        yield return new WaitUntil(() => MyComponents.PlayersSynchronisation.IsSynchronised(syncId));
        //
        MyComponents.MatchManager.StartGame();
    }

    [MyRPC]
    private void LoadRoom(short syncId)
    {
        Debug.Log("LoadRoom");
        this.syncId = syncId;
        NavigationManager.LoadScene(Scenes.Main, true, true);
        Players.MyPlayer.SceneChanged += SendReady;
        Debug.Log("EndLoadRoom");
    }

    //private void CreateTutorial(ConnectionId connectionId, short scene)
    //{
    //    GameObject tutorial = new GameObject("Tutorial");
    //    tutorial.transform.position = Vector3.forward * 1000f;
    //    tutorial.AddComponent<Tutorial>();
    //}

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
