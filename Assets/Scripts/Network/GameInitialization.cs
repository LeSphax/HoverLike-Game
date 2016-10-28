using Byn.Net;
using Navigation;
using PlayerManagement;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class GameInitialization : SlideBall.MonoBehaviour
{
    bool started = false;

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
        MyGameObjects.NetworkManagement.NewPlayerConnectedToRoom += UpdateNumberPlayers;
    }

    private void UpdateNumberPlayers(ConnectionId id)
    {
        MyGameObjects.Properties.SetProperty(PropertiesKeys.NumberPlayers, MyGameObjects.NetworkManagement.GetNumberPlayers());
    }


    [MyRPC]
    public void StartGame()
    {
        Assert.IsTrue(MyGameObjects.NetworkManagement.isServer);
        short[] teamSpawns = new short[2];
        foreach (Player player in Players.players.Values)
        {
            Assert.IsTrue(player.Team == Team.FIRST || player.Team == Team.SECOND);
            player.SpawnNumber = teamSpawns[(int)player.Team];
            if (player.SpawnNumber == 0)
            {
                player.AvatarSettingsType = AvatarSettings.AvatarSettingsTypes.GOALIE;
            }
            else
            {
                player.AvatarSettingsType = AvatarSettings.AvatarSettingsTypes.ATTACKER;

            }
            teamSpawns[(int)player.Team]++;
        }

        View.RPC("LoadRoom", RPCTargets.All);
    }

    private void SetupRoom()
    {
        InstantiateNewObjects();
        //Letting the time for the objects we just instantiated and the objects in the scene to call their Start method
        Invoke("GameHasStarted", 0.01f);
    }

    [MyRPC]
    private void LoadRoom()
    {
        NavigationManager.FinishedLoadingGame += () => { Players.MyPlayer.SceneId = Scenes.currentSceneId; };
        MyGameObjects.NetworkManagement.ReceivedAllBufferedMessages += SetupRoom;
        NavigationManager.LoadScene(Scenes.Main);
    }

    [MyRPC]
    private void InstantiateNewObjects()
    {
        Debug.Log("InstantiateNewObjects");
        if (MyGameObjects.NetworkManagement.isServer)
            MyGameObjects.NetworkViewsManagement.Instantiate("Ball", MyGameObjects.Spawns.BallSpawn, Quaternion.identity);

        GameObject player = MyGameObjects.NetworkViewsManagement.Instantiate("MyPlayer", new Vector3(0, 4.4f, 0), Quaternion.identity);
        int numberPlayer = MyGameObjects.Properties.GetProperty<int>(PropertiesKeys.NumberPlayers) - 1;
        player.GetComponent<PlayerController>().Init(Players.MyPlayer.id, numberPlayer % 2, "Player" + numberPlayer);
    }

    [MyRPC]
    private void GameHasStarted()
    {
        if (AllObjectsCreated != null)
            AllObjectsCreated.Invoke();
    }





}
