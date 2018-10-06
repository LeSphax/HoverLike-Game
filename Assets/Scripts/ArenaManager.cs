using Byn.Net;
using PlayerManagement;
using UnityEngine;

public class ArenaManager : MonoBehaviour, IGameInit
{
    public event EmptyEventHandler AllObjectsCreated;

    void Start()
    {
        Debug.Log("ArenaManager: CreatePlayer");
        Players.NewPlayerInstantiated += InitPlayer;

        MyComponents.NetworkViewsManagement.Instantiate("Ball", MyComponents.Spawns.BallSpawn, Quaternion.identity);

        ConnectionId id = ConnectionId.INVALID;
        Players.CreatePlayer(id);
        Players.myPlayerId = id;
        Players.MyPlayer.Nickname = UserSettings.Nickname;
        Players.MyPlayer.SceneId = Scenes.currentSceneId;

        if (AllObjectsCreated != null)
            AllObjectsCreated.Invoke();
    }

    private void InitPlayer(ConnectionId playerId)
    {
        Player player = Players.players[playerId];

        player.AvatarSettingsType = AvatarSettings.AvatarSettingsTypes.ATTACKER;
        player.Team = Team.BLUE;
        player.State.Movement = MovementState.PLAYING;
    }

    public void AddGameStartedListener(EmptyEventHandler handler)
    {
        handler.Invoke();
    }
}
