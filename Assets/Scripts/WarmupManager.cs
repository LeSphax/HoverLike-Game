using Byn.Net;
using PlayerManagement;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class WarmupManager : SlideBall.MonoBehaviour
{


    public void Activate(bool activate)
    {
        if (activate)
        {
            MyComponents.Players.players.Values.ForEach(player => { if (player.controller != null) InitPlayer(player.id); });
            MyComponents.Players.NewPlayerInstantiated += InitPlayer;
        }
        else
        {
            MyComponents.Players.players.Values.ForEach(player =>
            {
                player.eventNotifier.StopListeningToEvents(ChangeAvatar, PlayerFlags.PLAY_AS_GOALIE);
                player.eventNotifier.StopListeningToEvents(ResetPlayer, PlayerFlags.TEAM, PlayerFlags.AVATAR_SETTINGS);
            });
            MyComponents.Players.NewPlayerInstantiated -= InitPlayer;
        }
    }

    private void InitPlayer(ConnectionId playerId)
    {
        Player player = MyComponents.Players.players[playerId];

        if (NetworkingState.IsServer)
        {
            player.AvatarSettingsType = player.PlayAsGoalie ? AvatarSettings.AvatarSettingsTypes.GOALIE : AvatarSettings.AvatarSettingsTypes.ATTACKER;
            if (player.Team == Team.NONE)
                player.Team = GetInitialTeam();
            player.eventNotifier.ListenToEvents(ChangeAvatar, PlayerFlags.PLAY_AS_GOALIE);
        }

        player.eventNotifier.ListenToEvents(ResetPlayer, PlayerFlags.TEAM, PlayerFlags.AVATAR_SETTINGS);

        ResetPlayer(player);
    }

    private void ChangeAvatar(Player player)
    {
        player.AvatarSettingsType = player.PlayAsGoalie ? AvatarSettings.AvatarSettingsTypes.GOALIE : AvatarSettings.AvatarSettingsTypes.ATTACKER;
    }

    private void ResetPlayer(Player player)
    {
        if (NetworkingState.IsServer)
        {
            player.SpawnNumber = -1;
            var takenSpawningPoints = MyComponents.Players.GetPlayersInTeam(player.Team).Select(p => p.SpawnNumber);
            short preferredSpawnNumber = player.AvatarSettingsType == AvatarSettings.AvatarSettingsTypes.GOALIE ? (short)0 : (short)1;
            while (takenSpawningPoints.Contains(preferredSpawnNumber))
                preferredSpawnNumber++;
            player.SpawnNumber = preferredSpawnNumber;
            player.State.Movement = MovementState.PLAYING;
        }

        player.controller.ResetPlayer();


    }

    private Team GetInitialTeam()
    {
        Assert.IsTrue(NetworkingState.IsServer);
        Team team;
        if  (MyComponents.Players.GetPlayersInTeam(Team.BLUE).Count <= MyComponents.Players.GetPlayersInTeam(Team.RED).Count)
            team = Team.BLUE;
        else
            team = Team.RED;
        return team;
    }

    private void OnDestroy()
    {
        Activate(false);
    }
}