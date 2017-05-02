using System;
using SlideBall;
using Navigation;
using PlayerManagement;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public static class MyComponents
{

    public static void NullifyComponents()
    {
        if (Scenes.IsCurrentScene(Scenes.LobbyIndex))
        {
            NullifyMain();
            NullifyRoom();
        }
        else if (Scenes.IsCurrentScene(Scenes.MainIndex))
        {
            NullifyLobby();
            NullifyRoom();
        }
        else if (Scenes.IsCurrentScene(Scenes.RoomIndex))
        {
            NullifyLobby();
            NullifyMain();
        }
    }

    private static void NullifyLobby()
    {
        lobbyManager = null;
    }

    private static void NullifyRoom()
    {
        roomManager = null;
        chatManager = null;
    }

    private static void NullifyMain()
    {
        matchManager = null;
        abilitiesFactory = null;
        ballState = null;
        spawns = null;
        victoryPose = null;
        victoryUI = null;
        chatManager = null;
        battleriteCamera = null;
    }

    private static NetworkManagement networkManagement;
    public static NetworkManagement NetworkManagement
    {
        get
        {
            if (networkManagement == null)
            {
                GameObject go = GameObject.FindGameObjectWithTag(Tags.NetworkScripts);
                if (go != null)
                    networkManagement = go.GetComponent<NetworkManagement>();
            }
            return networkManagement;
        }
    }

    private static PopUp popUp;
    public static PopUp PopUp
    {
        get
        {
            if (popUp == null)
            {
                GameObject go;
                if ((go = GameObject.FindGameObjectWithTag(Tags.PopUp)) != null)
                    popUp = go.GetComponentInChildren<PopUp>();
            }
            return popUp;
        }
    }

    private static VictoryPose victoryPose;
    public static VictoryPose VictoryPose
    {
        get
        {
            if (victoryPose == null)
            {
                victoryPose = GetTaggedComponent<VictoryPose>(Tags.VictoryPose);
            }
            return victoryPose;
        }
    }

    private static VictoryUI victoryUI;
    public static VictoryUI VictoryUI
    {
        get
        {
            if (victoryUI == null)
            {
                victoryUI = GetTaggedComponent<VictoryUI>(Tags.VictoryUI);
            }
            return victoryUI;
        }
    }

    private static PlayersSynchronisation playersSynchronisation;
    public static PlayersSynchronisation PlayersSynchronisation
    {
        get
        {
            if (playersSynchronisation == null)
            {
                GameObject go;
                if ((go = GameObject.FindGameObjectWithTag(Tags.NetworkScripts)) != null)
                    playersSynchronisation = go.GetComponent<PlayersSynchronisation>();
            }
            return playersSynchronisation;
        }
    }

    private static AbilitiesFactory abilitiesFactory;
    public static AbilitiesFactory AbilitiesFactory
    {
        get
        {
            Assert.IsTrue(Scenes.IsCurrentScene(Scenes.MainIndex));
            if (abilitiesFactory == null)
            {
                abilitiesFactory = GameObject.FindGameObjectWithTag(Tags.Abilities).GetComponent<AbilitiesFactory>();
            }
            return abilitiesFactory;
        }
    }

    private static BallState ballState;
    public static BallState BallState
    {
        get
        {
            //Assert.IsTrue(Scenes.IsCurrentScene(Scenes.MainIndex));
            if (ballState == null)
            {
                GameObject ball = GameObject.FindGameObjectWithTag(Tags.Ball);
                if (ball != null)
                    ballState = ball.GetComponent<BallState>();
            }
            return ballState;
        }
    }

    private static MatchManager matchManager;
    public static MatchManager MatchManager
    {
        get
        {
            //Assert.IsTrue(Scenes.IsCurrentScene(Scenes.MainIndex));
            if (matchManager == null)
            {
                GameObject go = GameObject.FindGameObjectWithTag(Tags.Room);
                if (go != null)
                    matchManager = go.GetComponent<MatchManager>();
            }
            return matchManager;
        }
    }

    private static Players players;
    public static Players Players
    {
        get
        {
            if (players == null)
            {
                players = GameObject.FindGameObjectWithTag(Tags.NetworkScripts).GetComponent<Players>();
            }
            return players;
        }
    }

    private static TimeManagement timeManagement;
    public static TimeManagement TimeManagement
    {
        get
        {
            if (timeManagement == null)
            {
                GameObject go;
                if ((go = GameObject.FindGameObjectWithTag(Tags.NetworkScripts)) != null)
                    timeManagement = go.GetComponent<TimeManagement>();
            }
            return timeManagement;
        }
    }

    private static LobbyManager lobbyManager;
    public static LobbyManager LobbyManager
    {
        get
        {
            Assert.IsTrue(Scenes.IsCurrentScene(Scenes.LobbyIndex));
            if (lobbyManager == null)
            {
                lobbyManager = GameObject.FindGameObjectWithTag(Tags.LobbyManager).GetComponent<LobbyManager>();
            }
            return lobbyManager;
        }
    }

    private static RoomManager roomManager;
    public static RoomManager RoomManager
    {
        get
        {
            Assert.IsTrue(Scenes.IsCurrentScene(Scenes.RoomIndex));
            if (roomManager == null)
            {
                roomManager = GameObject.FindGameObjectWithTag(Tags.Room).GetComponent<RoomManager>();
            }
            return roomManager;
        }
    }

    private static NetworkViewsManagement networkViewsManagement;
    public static NetworkViewsManagement NetworkViewsManagement
    {
        get
        {
            GameObject go;
            if (networkViewsManagement == null && (go = GameObject.FindGameObjectWithTag(Tags.NetworkScripts)) != null)
            {
                networkViewsManagement = go.GetComponent<NetworkViewsManagement>();
            }
            return networkViewsManagement;
        }
    }

    private static GameInitialization gameInitialization;
    public static GameInitialization GameInitialization
    {
        get
        {
            if (gameInitialization == null)
            {
                gameInitialization = GameObject.FindGameObjectWithTag(Tags.NetworkScripts).GetComponent<GameInitialization>();
            }
            return gameInitialization;
        }
    }

    private static ChatManager chatManager;
    public static ChatManager ChatManager
    {
        get
        {
            if (chatManager == null)
            {
                GameObject go = GameObject.FindGameObjectWithTag(Tags.NetworkScripts);
                if (go != null)
                    chatManager = go.GetComponent<ChatManager>();
            }
            return chatManager;
        }
    }

    private static Spawns spawns;
    public static Spawns Spawns
    {
        get
        {
            Assert.IsTrue(Scenes.IsCurrentScene(Scenes.MainIndex));
            if (spawns == null)
            {
                spawns = GameObject.FindGameObjectWithTag(Tags.Spawns).GetComponent<Spawns>();
            }
            return spawns;
        }
    }

    private static GlobalSound globalSound;
    public static GlobalSound GlobalSound
    {
        get
        {
            if (globalSound == null)
            {
                GameObject go = GameObject.FindGameObjectWithTag(Tags.GlobalSound);
                if (go != null)
                    globalSound = go.GetComponent<GlobalSound>();
            }
            return globalSound;
        }
    }

    private static BattleriteCamera battleriteCamera;
    public static BattleriteCamera BattleriteCamera
    {
        get
        {
            Assert.IsTrue(Scenes.IsCurrentScene(Scenes.MainIndex));
            if (battleriteCamera == null)
            {
                GameObject go = GameObject.FindGameObjectWithTag(Tags.BattleriteCamera);
                if (go != null)
                    battleriteCamera = go.GetComponent<BattleriteCamera>();
            }
            return battleriteCamera;
        }
    }

    public static GameObject UI()
    {
        return GameObject.FindGameObjectWithTag(Tags.UI);
    }

    public static Rigidbody MyPlayerRigidbody()
    {
        return GetTaggedComponent<Rigidbody>(Tags.MyPlayer);
    }

    private static Type GetTaggedComponent<Type>(string tag)
    {
        GameObject go = GameObject.FindGameObjectWithTag(tag);
        if (go != null)
            return go.GetComponent<Type>();
        return default(Type);
    }

    public static void ResetNetworkComponents()
    {
        Debug.Log("Reset Network Components");
        NavigationManager.Reset();
        Players.Reset();
        NetworkViewsManagement.PartialReset();
        NetworkManagement.Reset();
        ResetGameComponents();
        TimeManagement.Reset();
    }

    public static void ResetGameComponents()
    {
        GameInitialization.Reset();
        PlayersSynchronisation.Reset();
    }
}


