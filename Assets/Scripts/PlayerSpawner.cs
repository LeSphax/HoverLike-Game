using AbilitiesManagement;
using Byn.Net;
using PlayerManagement;
using UnityEngine;

public class PlayerSpawner : SlideBall.MonoBehaviour
{
    [MyRPC]
    public void DesactivatePlayers()
    {
        Players.players.Values.Map(player =>
        {
            if (player.controller != null)
                player.controller.gameObject.SetActive(false);
        });
    }

    [MyRPC]
    public void ResetPlayers()
    {
        Players.players.Values.Map(player =>
        {
            if (player.controller != null)
                player.controller.ResetPlayer();
        });
    }

    [MyRPC]
    public void ReactivatePlayers(short syncId)
    {
        Players.players.Values.Map(player =>
        {
            if (player.controller != null)
                player.controller.gameObject.SetActive(true);
        });
    }

}
