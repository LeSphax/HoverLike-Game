using AbilitiesManagement;
using Byn.Net;
using PlayerManagement;
using UnityEngine;

public class PlayerSpawner : SlideBall.MonoBehaviour
{
    [MyRPC]
    public void DesactivatePlayers(short syncId)
    {
        Players.players.Values.Map(player =>
        {
            if (player.controller != null)
                player.controller.gameObject.SetActive(false);
        });
        MyComponents.PlayersSynchronisation.SendSynchronisation(syncId);
    }

    [MyRPC]
    public void ResetPlayers(short syncId)
    {
        Players.players.Values.Map(player =>
        {
            if (player.controller != null)
                player.controller.ResetPlayer();
        });

        MyComponents.PlayersSynchronisation.SendSynchronisation(syncId);
    }

    [MyRPC]
    public void ReactivatePlayers(short syncId)
    {
        Players.players.Values.Map(player =>
        {
            if (player.controller != null)
                player.controller.gameObject.SetActive(true);
        });
        MyComponents.PlayersSynchronisation.SendSynchronisation(syncId);
    }

}
