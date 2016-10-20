using Byn.Net;

public class PlayerView : SlideBall.MonoBehaviour
{
    public ConnectionId connectionId;

    public Player Player
    {
        get
        {
            if (View.isMine)
                return Players.MyPlayer;
            else
                return Players.players[connectionId];
        }
    }
}