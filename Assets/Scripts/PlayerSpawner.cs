using AbilitiesManagement;
using Byn.Net;
using PlayerManagement;
using UnityEngine;

public class PlayerSpawner : SlideBall.NetworkMonoBehaviour
{
    [MyRPC]
    public void DesactivatePlayers()
    {
       MyComponents.Players.players.Values.Map(player =>
        {
            if (player.controller != null)
                player.controller.gameObject.SetActive(false);
        });
    }

    [MyRPC]
    public void ResetPlayers()
    {
       MyComponents.Players.players.Values.Map(player =>
        {
            if (player.controller != null)
                player.controller.ResetPlayer();
        });
    }

    [MyRPC]
    public void ReactivatePlayers()
    {
       MyComponents.Players.players.Values.Map(player =>
        {
            if (player.controller != null)
                player.controller.gameObject.SetActive(true);
        });
    }

    public void Reset()
    {
        DesactivatePlayers();
        ResetPlayers();
        ReactivatePlayers();
    }

}
