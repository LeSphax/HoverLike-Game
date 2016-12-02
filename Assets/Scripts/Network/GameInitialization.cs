﻿using Byn.Net;
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
        StartCoroutine(CoStartGame());
    }

    public IEnumerator CoStartGame()
    {
        Assert.IsTrue(MyComponents.NetworkManagement.isServer);

        short syncId = MyComponents.PlayersSynchronisation.GetNewSynchronisationId();
        View.RPC("LoadRoom", RPCTargets.All, syncId);
        yield return new WaitUntil(() => MyComponents.PlayersSynchronisation.IsSynchronised(syncId));
        //
        syncId = MyComponents.PlayersSynchronisation.GetNewSynchronisationId();
        InstantiateNewObjects();
        View.RPC("SendReady", RPCTargets.All, syncId);
        yield return new WaitUntil(() => MyComponents.PlayersSynchronisation.IsSynchronised(syncId));
        //
        MyComponents.MatchManager.StartGame();
    }

    [MyRPC]
    private void LoadRoom(short syncId)
    {
        this.syncId = syncId;
        NavigationManager.LoadScene(Scenes.Main);
        Players.MyPlayer.SceneChanged += SendReady;
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
            GameObject player = MyComponents.NetworkViewsManagement.Instantiate("MyPlayer", new Vector3(0, 4.4f, 0), Quaternion.identity);
            player.GetComponent<PlayerController>().Init(id);
        }
    }

    private void SendReady(ConnectionId id, short newSceneId)
    {
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
    }
}
