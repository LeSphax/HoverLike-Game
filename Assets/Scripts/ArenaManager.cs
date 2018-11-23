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

        CreatePlayer(ConnectionId.INVALID, "Alice", Team.BLUE, true);
        CreatePlayer(new ConnectionId(1), "Bob", Team.RED);

        if (AllObjectsCreated != null)
            AllObjectsCreated.Invoke();

            UserSettings.Volume = 0;
    }

    private void CreatePlayer(ConnectionId id, string nickname, Team team, bool isMyPlayer = false)
    {
        MyComponents.Players.CreatePlayer(id);
        if (isMyPlayer)
            MyComponents.Players.MyPlayerId = id;

        Player player = MyComponents.Players.players[id];
        player.Nickname = nickname;
        player.SceneId = Scenes.currentSceneId;
        player.team = team;
        player.controller.ResetPlayer();
    }

    private void InitPlayer(ConnectionId playerId)
    {
        Player player = MyComponents.Players.players[playerId];

        player.avatarSettingsType = AvatarSettings.AvatarSettingsTypes.ATTACKER;
        player.State.Movement = MovementState.PLAYING;
    }

    public void AddGameStartedListener(EmptyEventHandler handler)
    {
        handler.Invoke();
    }
}
