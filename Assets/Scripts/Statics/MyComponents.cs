using Ball;
using Navigation;
using PlayerManagement;
using SlideBall;
using UnityEngine;
using UnityEngine.Assertions;

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
        //else if (Scenes.IsCurrentScene(Scenes.RoomIndex))
        //{
        //    NullifyLobby();
        //    NullifyMain();
        //}
    }

    private static void NullifyLobby()
    {
        lobbyManager = null;
    }

    private static void NullifyRoom()
    {
        chatManager = null;
    }

    private static void NullifyMain()
    {
        matchManager = null;
        warmupManager = null;
        abilitiesFactory = null;
        ballState = null;
        spawns = null;
        victoryPose = null;
        victoryUI = null;
        chatManager = null;
        battleriteCamera = null;
        inputManager = null;
    }

    private static ANetworkManagement networkManagement;
    public static ANetworkManagement NetworkManagement
    {
        get
        {
            return GetTaggedComponent(ref networkManagement, Tags.NetworkScripts, Scenes.Any);
        }
    }

    private static GameplaySettings gameplaySettings;
    public static GameplaySettings GameplaySettings
    {
        get
        {
            return GetTaggedComponent(ref gameplaySettings, Tags.GameplaySettings, Scenes.Any);
        }
    }

    private static PopUp popUp;
    public static PopUp PopUp
    {
        get
        {
            return GetTaggedComponent(ref popUp, Tags.PopUp, Scenes.Any);
        }
    }

    private static VictoryPose victoryPose;
    public static VictoryPose VictoryPose
    {
        get
        {
            return GetTaggedComponent(ref victoryPose, Tags.VictoryPose, Scenes.MainIndex);
        }
    }

    private static VictoryUI victoryUI;
    public static VictoryUI VictoryUI
    {
        get
        {
            return GetTaggedComponent(ref victoryUI, Tags.VictoryUI, Scenes.MainIndex);
        }
    }

    private static PlayersSynchronisation playersSynchronisation;
    public static PlayersSynchronisation PlayersSynchronisation
    {
        get
        {
            return GetTaggedComponent(ref playersSynchronisation, Tags.NetworkScripts, Scenes.Any);
        }
    }

    private static AbilitiesFactory abilitiesFactory;
    public static AbilitiesFactory AbilitiesFactory
    {
        get
        {
            return GetTaggedComponent(ref abilitiesFactory, Tags.Abilities, Scenes.MainIndex);
        }
    }

    private static BallState ballState;
    public static BallState BallState
    {
        get
        {
            return GetTaggedComponent(ref ballState, Tags.Ball, Scenes.MainIndex);
        }
    }

    private static MatchManager matchManager;
    public static MatchManager MatchManager
    {
        get
        {
            return GetTaggedComponent(ref matchManager, Tags.Room, Scenes.MainIndex);
        }
    }

    private static WarmupManager warmupManager;
    public static WarmupManager WarmupManager
    {
        get
        {
            return GetTaggedComponent(ref warmupManager, Tags.Room, Scenes.MainIndex);
        }
    }

    private static Players players;
    public static Players Players
    {
        get
        {
            return GetTaggedComponent(ref players, Tags.NetworkScripts, Scenes.Any);
        }
    }

    private static TimeManagement timeManagement;
    public static TimeManagement TimeManagement
    {
        get
        {
            return GetTaggedComponent(ref timeManagement, Tags.NetworkScripts, Scenes.Any);
        }
    }

    private static LobbyManager lobbyManager;
    public static LobbyManager LobbyManager
    {
        get
        {
            return GetTaggedComponent(ref lobbyManager, Tags.LobbyManager, Scenes.LobbyIndex);
        }
    }

    //private static RoomManager roomManager;
    //public static RoomManager RoomManager
    //{
    //    get
    //    {
    //        Assert.IsTrue(Scenes.IsCurrentScene(Scenes.RoomIndex));
    //        if (roomManager == null)
    //        {
    //            roomManager = GameObject.FindGameObjectWithTag(Tags.Room).GetComponent<RoomManager>();
    //        }
    //        return roomManager;
    //    }
    //}

    private static NetworkViewsManagement networkViewsManagement;
    public static NetworkViewsManagement NetworkViewsManagement
    {
        get
        {
            return GetTaggedComponent(ref networkViewsManagement, Tags.NetworkScripts, Scenes.Any);
        }
    }

    private static GameState gameState;
    public static GameState GameState
    {
        get
        {
            return GetTaggedComponent(ref gameState, Tags.NetworkScripts, Scenes.Any);

        }
    }

    private static IGameInit gameInit;
    public static IGameInit GameInit
    {
        get
        {
            return GetTaggedComponent(ref gameInit, Tags.NetworkScripts, Scenes.Any);

        }
    }

    private static ChatManager chatManager;
    public static ChatManager ChatManager
    {
        get
        {
            return GetTaggedComponent(ref chatManager, Tags.NetworkScripts, Scenes.Any);
        }
    }

    private static Spawns spawns;
    public static Spawns Spawns
    {
        get
        {
            return GetTaggedComponent(ref spawns, Tags.Spawns, Scenes.MainIndex);
        }
    }

    private static GlobalSound globalSound;
    public static GlobalSound GlobalSound
    {
        get
        {
            return GetTaggedComponent(ref globalSound, Tags.GlobalSound, Scenes.Any);
        }
    }

    private static BattleriteCamera battleriteCamera;
    public static BattleriteCamera BattleriteCamera
    {
        get
        {
            return GetTaggedComponent(ref battleriteCamera, Tags.BattleriteCamera, Scenes.MainIndex);
        }
    }

    private static InputManager inputManager;
    public static InputManager InputManager
    {
        get
        {
            return GetTaggedComponent(ref inputManager, Tags.Room, Scenes.MainIndex);
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

    private static Type GetTaggedComponent<Type>(ref Type cache, string tag, int sceneIndex)
    {
        if (sceneIndex != Scenes.Any)
        {
            Assert.IsTrue(Scenes.IsCurrentScene(sceneIndex));
        }
        if (cache == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag(tag);
            if (go != null)
                cache = go.GetComponent<Type>();
        }
        return cache;
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
        GameState.Reset();
        PlayersSynchronisation.Reset();
    }
}


