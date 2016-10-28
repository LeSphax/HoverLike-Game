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
            return GetMyPlayer(View.isMine, playerConnectionId);
        }
    }

    public static Player GetMyPlayer(bool hasOwnView, ConnectionId connectionId)
    {
        if (hasOwnView)
            return Players.MyPlayer;
        else
            return Players.players[connectionId];
    }
}