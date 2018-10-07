using Byn.Net;
using PlayerManagement;
using UnityEngine;

public class PlayerView : SlideBall.NetworkMonoBehaviour
{
    public ConnectionId playerConnectionId = Players.INVALID_PLAYER_ID;

    public Player Player
    {
        get
        {
            return GetMyPlayer(playerConnectionId);
        }
    }

    public Player GetMyPlayer(ConnectionId connectionId)
    {
        Player player;
        if (!MyComponents.Players.players.TryGetValue(connectionId, out player))
            return null;
        return player;
    }
}