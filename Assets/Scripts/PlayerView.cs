using Byn.Net;
using PlayerManagement;
using UnityEngine;

public class PlayerView : SlideBall.NetworkMonoBehaviour
{
    private ConnectionId playerConnectionId = Players.INVALID_PLAYER_ID;
    public ConnectionId PlayerConnectionId{
        get{
            return playerConnectionId;
        }
        set{
            playerConnectionId = value;
            MyComponents.Players.players.TryGetValue(playerConnectionId, out Player);
        }
    }

    public Player Player;
}