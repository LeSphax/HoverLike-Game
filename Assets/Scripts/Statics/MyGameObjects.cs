using BaseNetwork;
using PlayerManagement;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public static class MyComponents
{
    private static NetworkManagement networkManagement;
    public static NetworkManagement NetworkManagement
    {
        get
        {
            if (networkManagement == null)
            {
                GameObject go;
                if ((go = GameObject.FindGameObjectWithTag(Tags.NetworkScripts)) != null)
                    networkManagement = go.GetComponent<NetworkManagement>();
            }
            return networkManagement;
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
            Assert.IsTrue(Scenes.IsCurrentScene(Scenes.MainIndex));
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
            Assert.IsTrue(Scenes.IsCurrentScene(Scenes.MainIndex));
            if (matchManager == null)
            {
                matchManager = GameObject.FindGameObjectWithTag(Tags.Room).GetComponent<MatchManager>();
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
                timeManagement = GameObject.FindGameObjectWithTag(Tags.NetworkScripts).GetComponent<TimeManagement>();
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

    private static NetworkViewsManagement networkViewsManagement;
    public static NetworkViewsManagement NetworkViewsManagement
    {
        get
        {
            if (networkViewsManagement == null)
            {
                networkViewsManagement = GameObject.FindGameObjectWithTag(Tags.NetworkScripts).GetComponent<NetworkViewsManagement>();
            }
            return networkViewsManagement;
        }
    }

    private static NetworkProperties properties;
    public static NetworkProperties Properties
    {
        get
        {
            if (properties == null)
            {
                properties = GameObject.FindGameObjectWithTag(Tags.NetworkScripts).GetComponent<NetworkProperties>();
            }
            return properties;
        }
    }

    private static RoomManager roomManager;
    public static RoomManager RoomManager
    {
        get
        {
            Assert.IsTrue(Scenes.IsCurrentScene(Scenes.LobbyIndex));
            if (roomManager == null)
            {
                roomManager = GameObject.FindGameObjectWithTag(Tags.Room).GetComponent<RoomManager>();
            }
            return roomManager;
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

    private static Countdown countdown;
    public static Countdown Countdown
    {
        get
        {
            Assert.IsTrue(Scenes.IsCurrentScene(Scenes.MainIndex));
            if (countdown == null)
            {
                countdown = GameObject.FindGameObjectWithTag(Tags.Countdown).GetComponent<Countdown>();
            }
            return countdown;
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

}


