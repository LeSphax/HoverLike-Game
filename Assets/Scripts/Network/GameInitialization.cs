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

    public bool isGoal;

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
        MyComponents.NetworkManagement.NewPlayerConnectedToRoom += UpdateNumberPlayers;
    }

    private void UpdateNumberPlayers(ConnectionId id)
    {
        MyComponents.Properties.SetProperty(PropertiesKeys.NumberPlayers, MyComponents.NetworkManagement.GetNumberPlayers());
    }

    public void StartGame()
    {
        StartCoroutine(CoStartGame());
    }

    public IEnumerator CoStartGame()
    {
        Assert.IsTrue(MyComponents.NetworkManagement.isServer);
        short[] teamSpawns = new short[2] { 1, 1 };
        foreach (Player player in Players.players.Values)
        {
            Assert.IsTrue(player.Team == Team.FIRST || player.Team == Team.SECOND);
            player.SpawnNumber = teamSpawns[(int)player.Team];
            if (player.SpawnNumber == 0 && isGoal)
            {
                player.AvatarSettingsType = AvatarSettings.AvatarSettingsTypes.GOALIE;
            }
            else
            {
                player.AvatarSettingsType = AvatarSettings.AvatarSettingsTypes.ATTACKER;
            }
            teamSpawns[(int)player.Team]++;
        }

        short syncId = MyComponents.PlayersSynchronisation.GetNewSynchronisationId();
        View.RPC("LoadRoom", RPCTargets.All, syncId);
        yield return new WaitUntil(() => MyComponents.PlayersSynchronisation.IsSynchronised(syncId));
        AllObjectsCreated.Invoke();
        MyComponents.MatchManager.StartGameCountdown();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("SPACE" + MyComponents.PlayersSynchronisation.IsSynchronised(syncId) + "   " + syncId);
        }
    }

    private void SetupRoom()
    {
        Debug.Log("SetupRoom");
        MyComponents.NetworkManagement.ReceivedAllBufferedMessages -= SetupRoom;
        InstantiateNewObjects();
        //Letting the time for the objects we just instantiated and the objects in the scene to call their Start method
        Invoke("GameHasStarted", 0.01f);
    }

    [MyRPC]
    private void LoadRoom(short syncId)
    {
        Debug.Log("LoadRoom");
        this.syncId = syncId;
        MyComponents.NetworkManagement.ReceivedAllBufferedMessages += SetupRoom;
        NavigationManager.LoadScene(Scenes.Main);
    }

    private void InstantiateNewObjects()
    {
        Debug.Log("InstantiateNewObjects");
        if (MyComponents.NetworkManagement.isServer)
            MyComponents.NetworkViewsManagement.Instantiate("Ball", Vector3.zero, Quaternion.identity);

        GameObject player = MyComponents.NetworkViewsManagement.Instantiate("MyPlayer2", new Vector3(0, 4.4f, 0), Quaternion.identity);
        int numberPlayer = MyComponents.Properties.GetProperty<int>(PropertiesKeys.NumberPlayers) - 1;
        player.GetComponent<PlayerController>().Init(Players.MyPlayer.id, numberPlayer % 2, "Player" + numberPlayer);
    }

    private void GameHasStarted()
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
