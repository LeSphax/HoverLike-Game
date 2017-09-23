using Byn.Net;
using PlayerManagement;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class WarmupManager : MonoBehaviour
{


    public void Activate(bool activate)
    {
        if (activate)
        {
            Debug.LogError("Activate");
            Players.players.Values.ForEach(player => { Debug.LogError("ActivateP"); if (player.controller != null) InitPlayer(player.id); });
            Players.NewPlayerInstantiated += InitPlayer;
        }
        else
        {
            Players.players.Values.ForEach(player =>
            {
                player.eventNotifier.StopListeningToEvents(ChangeAvatar, PlayerFlags.PLAY_AS_GOALIE);
                player.eventNotifier.StopListeningToEvents(ResetPlayer, PlayerFlags.TEAM, PlayerFlags.AVATAR_SETTINGS);
            });
            Players.NewPlayerInstantiated -= InitPlayer;
        }
    }

    private void InitPlayer(ConnectionId playerId)
    {
        Debug.LogError("Init");
        Player player = Players.players[playerId];

        if (MyComponents.NetworkManagement.IsServer)
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
        if (MyComponents.NetworkManagement.IsServer)
        {
            player.SpawnNumber = -1;
            var takenSpawningPoints = Players.GetPlayersInTeam(player.Team).Select(p => p.SpawnNumber);
            short preferredSpawnNumber = player.AvatarSettingsType == AvatarSettings.AvatarSettingsTypes.GOALIE ? (short)0 : (short)1;
            while (takenSpawningPoints.Contains(preferredSpawnNumber))
                preferredSpawnNumber++;
            player.SpawnNumber = preferredSpawnNumber;
            player.CurrentState = Player.State.PLAYING;
        }

        player.controller.ResetPlayer();


    }

    private static Team GetInitialTeam()
    {
        Assert.IsTrue(MyComponents.NetworkManagement.IsServer);
        Team team;
        if (Players.GetPlayersInTeam(Team.BLUE).Count <= Players.GetPlayersInTeam(Team.RED).Count)
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