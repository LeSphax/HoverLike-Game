using UnityEngine;

public class BallState : MonoBehaviour
{
    //This is set to false when we want the ball's simulation to be handled by the client
    public static bool ListenToServer = true;
    public static GameObject ball;
    private static Vector3 ballHoldingPosition = new Vector3(.5f, .5f, .5f);


    void Awake()
    {
        ball = gameObject;
        MyGameObjects.RoomManager.AddGameStartedListener(StartGame);
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

        AttachBall(GetAttachedPlayerID());

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
        return GetAttachedPlayerID() != -1;
    }

    public static int GetAttachedPlayerID()
    {
        object attachedPlayerID;
        MyGameObjects.Properties.TryGetProperty(PropertiesKeys.NamePlayerHoldingBall, out attachedPlayerID);
        if (attachedPlayerID == null)
        {
            //Debug.LogError("The attachedPlayer wasn't set, this should not happen");
            return -1;
        }
        else
            return (int)attachedPlayerID;
    }

    public static GameObject GetAttachedPlayer()
    {
        foreach (GameObject player in Tags.FindPlayers())
        {
            if (player.GetNetworkView().ViewId == GetAttachedPlayerID())
                return player;
        }
        return null;
    }


    public static void AttachBall(int viewId)
    {
        bool attach = viewId != -1;
        GameObject player = GetAttachedPlayer();
        if (attach)
        {
            Debug.Log("Attach Ball" + ball + "    " + player);
            ball.transform.SetParent(player.transform);
            ball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            ball.transform.localPosition = ballHoldingPosition;
        }
        else
        {
            Debug.Log("Detach Ball");
            //Physics.IgnoreCollision(ball.GetComponent<Collider>(), player.GetComponent<Collider>(),);
            ball.transform.SetParent(null);
            ball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }
}
