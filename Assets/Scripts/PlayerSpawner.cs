using Byn.Net;
using PlayerManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpawner : SlideBall.MonoBehaviour
{
    [MyRPC]
    public void DesactivatePlayers(short syncId)
    {
        Players.players.Values.Map(player =>
        {
            player.controller.gameObject.SetActive(false);
        });
        MyComponents.PlayersSynchronisation.SendSynchronisation(syncId);
    }

    [MyRPC]
    public void ResetPlayers(short syncId)
    {
        Players.players.Values.Map(player =>
        {
            player.controller.ResetPlayer();
        });

        MyComponents.PlayersSynchronisation.SendSynchronisation(syncId);
    }

    [MyRPC]
    public void ReactivatePlayers(short syncId)
    {
        Players.players.Values.Map(player =>
        {
            player.controller.gameObject.SetActive(true);
        });
        MyComponents.PlayersSynchronisation.SendSynchronisation(syncId);
    }

}
