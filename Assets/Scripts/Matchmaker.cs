using System;
using Byn.Net;
using UnityEngine;
using UnityEngine.Assertions;

public class MatchMaker : ANetworkView
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
        MyGameObjects.NetworkManagement.ServerCreated += ServerStartGame;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MyGameObjects.NetworkManagement.PrintViews();
        }
    }

    void ServerStartGame()
    {
        Debug.Log("ServerStart");
        if (MyGameObjects.NetworkManagement.isServer)
        {
            MyNetworkView.Instantiate("Ball", new Vector3(10, 10, 10), Quaternion.identity);
        }
        StartGame();
    }

    private void StartGame()
    {
        if (!started)
        {
            started = true;
            GameObject player = MyNetworkView.Instantiate("MyPlayer", new Vector3(0, 4.4f, 0), Quaternion.identity);
            int numberPlayer = MyGameObjects.Properties.GetProperty<int>(PropertiesKeys.NumberPlayers) - 1;
            player.GetComponent<PlayerController>().Init(numberPlayer % 2, "Player" + numberPlayer);
            Debug.Log("StartGame");
            GameStarted.Invoke();
        }
    }

    void ClientStartGame()
    {
        if (MyGameObjects.NetworkManagement.IsConnected)
        {
            StartGame();
        }
        else
        {
            Debug.LogError("The properties were updated before a new connection was made");
        }
    }

    public override void ReceiveNetworkMessage(ConnectionId id, NetworkMessage message)
    {
        Assert.IsTrue(message.type == MessageType.StartGame, "Received a "+ message.type +" message on MatchMaker");
        StartGame();
    }
}
