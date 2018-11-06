using Ball;
using Byn.Net;
using PlayerManagement;
using UnityEngine;

public class ArenaManager : SlideBall.MonoBehaviour, IGameInit
{
    public event EmptyEventHandler AllObjectsCreated;

    void Start()
    {
        MyComponents.Players.NewPlayerInstantiated += InitPlayer;

        MyComponents.NetworkViewsManagement.Instantiate("Ball", MyComponents.Spawns.BallSpawn, Quaternion.identity);

        ConnectionId id = ConnectionId.INVALID;
        MyComponents.Players.CreatePlayer(id);
        MyComponents.Players.myPlayerId = id;
        MyComponents.MyPlayer.Nickname = "Alice";
        MyComponents.MyPlayer.SceneId = Scenes.currentSceneId;


        //id = new ConnectionId(1);
        //MyComponents.Players.CreatePlayer(id);
        //MyComponents.Players.players[id].Nickname = "Bob";
        //MyComponents.Players.players[id].SceneId = Scenes.currentSceneId;
        //MyComponents.Players.players[id].Team = Team.RED;


        if (AllObjectsCreated != null)
            AllObjectsCreated.Invoke();
    }

    private void InitPlayer(ConnectionId playerId)
    {
        Player player = MyComponents.Players.players[playerId];

        player.AvatarSettingsType = AvatarSettings.AvatarSettingsTypes.ATTACKER;
        player.Team = Team.BLUE;
        player.State.Movement = MovementState.PLAYING;
    }

    public void AddGameStartedListener(EmptyEventHandler handler)
    {
        handler.Invoke();
    }
}
