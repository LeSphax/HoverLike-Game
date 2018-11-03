using Ball;
using Navigation;
using PlayerManagement;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(KeepMyComponents))]
public class MyComponents : MonoBehaviour
{
    public void NullifyComponents()
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
    }

    private void NullifyLobby()
    {
        lobbyManager = null;
    }

    private void NullifyRoom()
    {
        chatManager = null;
    }

    private void NullifyMain()
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
    }

    private ANetworkManagement networkManagement;
    public ANetworkManagement NetworkManagement
    {
        get
        {
            return GetTaggedComponent(ref networkManagement, Tags.NetworkScripts, Scenes.Any);
        }
    }

    private GameplaySettings gameplaySettings;
    public GameplaySettings GameplaySettings
    {
        get
        {
            return GetTaggedComponent(ref gameplaySettings, Tags.GameplaySettings, Scenes.Any);
        }
    }

    private PopUp popUp;
    public PopUp PopUp
    {
        get
        {
            return GetTaggedComponent(ref popUp, Tags.PopUp, Scenes.Any);
        }
    }

    private VictoryPose victoryPose;
    public VictoryPose VictoryPose
    {
        get
        {
            return GetTaggedComponent(ref victoryPose, Tags.VictoryPose, Scenes.MainIndex);
        }
    }

    private VictoryUI victoryUI;
    public VictoryUI VictoryUI
    {
        get
        {
            return GetTaggedComponent(ref victoryUI, Tags.VictoryUI, Scenes.MainIndex);
        }
    }

    private PlayersSynchronisation playersSynchronisation;
    public PlayersSynchronisation PlayersSynchronisation
    {
        get
        {
            return GetTaggedComponent(ref playersSynchronisation, Tags.NetworkScripts, Scenes.Any);
        }
    }

    private AbilitiesFactory abilitiesFactory;
    public AbilitiesFactory AbilitiesFactory
    {
        get
        {
            return GetTaggedComponent(ref abilitiesFactory, Tags.Abilities, Scenes.MainIndex);
        }
    }

    private BallState ballState;
    public BallState BallState
    {
        get
        {
            return GetTaggedComponent(ref ballState, Tags.Ball, Scenes.MainIndex);
        }
    }

    private MatchManager matchManager;
    public MatchManager MatchManager
    {
        get
        {
            return GetTaggedComponent(ref matchManager, Tags.Room, Scenes.MainIndex);
        }
    }

    private WarmupManager warmupManager;
    public WarmupManager WarmupManager
    {
        get
        {
            return GetTaggedComponent(ref warmupManager, Tags.Room, Scenes.MainIndex);
        }
    }

    private Players players;
    public Players Players
    {
        get
        {
            return GetTaggedComponent(ref players, Tags.NetworkScripts, Scenes.Any);
        }
    }

    public Player MyPlayer
    {
        get
        {
            return Players.MyPlayer;
        }
    }

    private TimeManagement timeManagement;
    public TimeManagement TimeManagement
    {
        get
        {
            return GetTaggedComponent(ref timeManagement, Tags.NetworkScripts, Scenes.Any);
        }
    }

    private LobbyManager lobbyManager;
    public LobbyManager LobbyManager
    {
        get
        {
            return GetTaggedComponent(ref lobbyManager, Tags.LobbyManager, Scenes.LobbyIndex);
        }
    }

    private NetworkViewsManagement networkViewsManagement;
    public NetworkViewsManagement NetworkViewsManagement
    {
        get
        {
            return GetTaggedComponent(ref networkViewsManagement, Tags.NetworkScripts, Scenes.Any);
        }
    }

    private GameState gameState;
    public GameState GameState
    {
        get
        {
            return GetTaggedComponent(ref gameState, Tags.NetworkScripts, Scenes.Any);

        }
    }

    private IGameInit gameInit;
    public IGameInit GameInit
    {
        get
        {
            return GetTaggedComponent(ref gameInit, Tags.NetworkScripts, Scenes.Any);

        }
    }

    private ChatManager chatManager;
    public ChatManager ChatManager
    {
        get
        {
            return GetTaggedComponent(ref chatManager, Tags.NetworkScripts, Scenes.Any);
        }
    }

    private Spawns spawns;
    public Spawns Spawns
    {
        get
        {
            return GetTaggedComponent(ref spawns, Tags.Spawns, Scenes.MainIndex);
        }
    }

    private GlobalSound globalSound;
    public GlobalSound GlobalSound
    {
        get
        {
            return GetTaggedComponent(ref globalSound, Tags.GlobalSound, Scenes.Any);
        }
    }

    private BattleriteCamera battleriteCamera;
    public BattleriteCamera BattleriteCamera
    {
        get
        {
            return GetTaggedComponent(ref battleriteCamera, Tags.BattleriteCamera, Scenes.MainIndex);
        }
    }

    public GameObject UI()
    {
        return transform.FindGameObjectWithTag(Tags.UI);
    }

    private Type GetTaggedComponent<Type>(ref Type cache, string tag, int sceneIndex)
    {
        if (sceneIndex != Scenes.Any)
        {
            Assert.IsTrue(Scenes.IsCurrentScene(sceneIndex));
        }

        if (cache == null)
        {
            GameObject go = transform.FindGameObjectWithTag(tag);
            if (go != null)
                cache = go.GetComponent<Type>();
        }
        return cache;
    }


    public void ResetNetworkComponents()
    {
        Debug.Log("Reset Network Components");
        NavigationManager.Reset();
        Players.Reset();
        NetworkViewsManagement.PartialReset();
        NetworkManagement.Reset();
        ResetGameComponents();
        TimeManagement.Reset();
    }

    public void ResetGameComponents()
    {
        GameState.Reset();
        PlayersSynchronisation.Reset();
    }
}


