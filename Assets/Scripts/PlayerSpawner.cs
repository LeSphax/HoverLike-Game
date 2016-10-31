using Byn.Net;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : SlideBall.MonoBehaviour
{
    public bool playersDesactivated;

    public void DesactivatePlayers()
    {
        if (MyGameObjects.NetworkManagement.isServer)
            View.RPC("DesactivatePlayers", RPCTargets.Others);
        Tags.FindPlayers().Map(player =>
        {
            player.SetActive(false);
        });
    }

    private void PlayersAreDesactivated(ConnectionId id)
    {

    }

    private static void SetPlayersRoles()
    {

        Tags.FindPlayers().Map(player =>
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            controller.Player.SpawnNumber = (short)((controller.Player.SpawnNumber + 1) % Players.GetNumberPlayersInTeam(controller.Player.Team));
            controller.Player.AvatarSettingsType = controller.Player.SpawnNumber == 0 ? AvatarSettings.AvatarSettingsTypes.GOALIE : AvatarSettings.AvatarSettingsTypes.ATTACKER;
            controller.View.RPC("ResetPlayer", RPCTargets.All);
        });
    }

}
