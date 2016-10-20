using Byn.Net;
using Navigation;
using UnityEngine;
using UnityEngine.Assertions;

public class GameInitialization : SlideBall.MonoBehaviour
{
    bool started = false;

    public event EmptyEventHandler GameStarted;

    public void AddGameStartedListener(EmptyEventHandler handler)
    {
        if (started)
            handler.Invoke();
        else
            GameStarted += handler;
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
        foreach(Player player in Players.players.Values)
        {
            Assert.IsTrue(player.Team == Team.FIRST || player.Team == Team.SECOND);
            player.SpawningPoint = teamSpawns[(int)player.Team];
        }

        View.RPC("LoadRoom", RPCTargets.All);
    }

    private void InvokeSetupRoom()
    {
        Invoke("SetupRoom", 0.001f);
    }

    private void SetupRoom()
    {
        InstantiateNewObjects();
        GameHasStarted();
    }

    [MyRPC]
    private void LoadRoom()
    {
        NavigationManager.FinishedLoadingGame += InvokeSetupRoom;
        NavigationManager.LoadScene(Scenes.Main);
    }

    [MyRPC]
    private void InstantiateNewObjects()
    {
        Debug.Log("InstantiateNewObjects");
        if (MyGameObjects.NetworkManagement.isServer)
            MyGameObjects.NetworkViewsManagement.Instantiate("Ball", new Vector3(10, 10, 10), Quaternion.identity);

        GameObject player = MyGameObjects.NetworkViewsManagement.Instantiate("MyPlayer", new Vector3(0, 4.4f, 0), Quaternion.identity);
        int numberPlayer = MyGameObjects.Properties.GetProperty<int>(PropertiesKeys.NumberPlayers) - 1;
        player.GetComponent<PlayerController>().Init(Players.MyPlayer.id,numberPlayer % 2, "Player" + numberPlayer);
    }

    [MyRPC]
    private void GameHasStarted()
    {
        GameStarted.Invoke();
    }





}
