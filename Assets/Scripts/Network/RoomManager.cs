using Byn.Net;
using Navigation;
using UnityEngine;
using UnityEngine.Assertions;

public class RoomManager : SlideBall.MonoBehaviour
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

    void Awake()
    {
        MyGameObjects.NetworkManagement.ServerCreated += CreateRoom;
        MyGameObjects.NetworkManagement.ConnectedToServer += CreateRoom;
    }

    void CreateRoom()
    {
        Debug.Log("Create Room on Server");
        Assert.IsTrue(MyGameObjects.NetworkManagement.isServer);
        LoadRoom();
    }

    private void SetupRoom()
    {
        MyGameObjects.Properties.SetProperty(PropertiesKeys.NumberPlayers, MyGameObjects.NetworkManagement.GetNumberPlayers());
        InstantiateNewObjects();
        StartGame();
    }

    [MyRPC]
    internal void SendRoomSetup(ConnectionId RPCSenderId)
    {
        Debug.Log("New Player Connected " + RPCSenderId);
        MyGameObjects.Properties.SetProperty(PropertiesKeys.NumberPlayers, MyGameObjects.NetworkManagement.GetNumberPlayers());
        SynchronizeData(RPCSenderId);
        View.RPC("InstantiateNewObjects", RPCSenderId);
        View.RPC("StartGame", RPCSenderId);
    }

    private void LoadRoom()
    {
        if (MyGameObjects.NetworkManagement.isServer)
        {
            NavigationManager.FinishedLoadingGame += SetupRoom;
        }
        else
        {
            NavigationManager.FinishedLoadingGame += AskForRoomSetup;
        }
        NavigationManager.LoadScene(Scenes.Main);
        //Debug.Log("Properties " + MyGameObjects.Properties);
    }

    private void AskForRoomSetup()
    {
        Assert.IsFalse(MyGameObjects.NetworkManagement.isServer);
        View.RPC("SendRoomSetup", RPCTargets.Server, null);
    }


    private void SynchronizeData(ConnectionId connectionId)
    {
        MyGameObjects.Properties.SendProperties();
        MyGameObjects.NetworkManagement.SendBufferedMessages(connectionId);
    }

    [MyRPC]
    private void InstantiateNewObjects()
    {
        Debug.Log("InstantiateNewObjects");
        if (MyGameObjects.NetworkManagement.isServer)
            MyGameObjects.NetworkViewsManagement.Instantiate("Ball", new Vector3(10, 10, 10), Quaternion.identity);

        GameObject player = MyGameObjects.NetworkViewsManagement.Instantiate("MyPlayer", new Vector3(0, 4.4f, 0), Quaternion.identity);
        int numberPlayer = MyGameObjects.Properties.GetProperty<int>(PropertiesKeys.NumberPlayers) - 1;
        player.GetComponent<PlayerController>().Init(numberPlayer % 2, "Player" + numberPlayer);
    }

    [MyRPC]
    private void StartGame()
    {
        GameStarted.Invoke();
    }



}
