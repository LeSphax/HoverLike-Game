using Byn.Net;
using PlayerManagement;
using UnityEngine;

public class PlayerView : SlideBall.MonoBehaviour
{
    public ConnectionId playerConnectionId;

    public Player Player
    {
        get
        {
            return GetMyPlayer(playerConnectionId);
        }
    }

    public static Player GetMyPlayer(ConnectionId connectionId)
    {
            Player player;
            if (!Players.players.TryGetValue(connectionId, out player))
                return null;
            return player;
    }
}