using UnityEngine;

public class BallState : MonoBehaviour
{
    //This is set to false when we want the ball's simulation to be handled by the client
    public static bool ListenToServer = true;
    void Awake()
    {
        MyGameObjects.MatchMaker.AddGameStartedListener(StartGame);
    }

    void Start()
    {
        MyGameObjects.Properties.AddListener(PropertiesKeys.NamePlayerHoldingBall, (previousValue, value) => ListenToServer = true);
    }

    public void StartGame()
    {
        Debug.Log("BallState : StartGame");
        if (MyGameObjects.NetworkManagement.isServer)
        {
            MyGameObjects.Properties.SetProperty(PropertiesKeys.NamePlayerHoldingBall, -1);
        }
        if (GetAttachedPlayerID() != -1)
        {
            GetAttachedPlayer().GetComponent<PlayerBallController>().AttachBall(false);
        }
    }

    public static void SetAttached(int playerID)
    {
        MyGameObjects.Properties.SetProperty(PropertiesKeys.NamePlayerHoldingBall, playerID);

    }
    public static void Detach()
    {
        MyGameObjects.Properties.SetProperty(PropertiesKeys.NamePlayerHoldingBall, -1);
    }

    public static bool IsAttached()
    {
        return MyGameObjects.Properties.GetProperty<int>(PropertiesKeys.NamePlayerHoldingBall) != -1;
    }

    public static int GetAttachedPlayerID()
    {
        object attachedPlayerID;
        MyGameObjects.Properties.TryGetProperty(PropertiesKeys.NamePlayerHoldingBall, out attachedPlayerID);
        if (attachedPlayerID == null)
            return -1;
        else
            return (int)attachedPlayerID;
    }

    public static GameObject GetAttachedPlayer()
    {
        foreach (GameObject player in Tags.FindPlayers())
        {
            if (player.GetNetworkView().viewId == GetAttachedPlayerID())
                return player;
        }
        return null;
    }

}
