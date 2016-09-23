using ExitGames.Client.Photon;
using UnityEngine;

public class BallState : Photon.MonoBehaviour
{

    private static GameObject ball;

    void Start()
    {
        ball = gameObject;
        if (!PhotonNetwork.room.customProperties.ContainsKey(NetworkRoomKeys.NamePlayerHoldingBall))
        {
            Detach();
        }
        if (GetAttachedPlayerID() != -1)
        {
            GetAttachedPlayer().GetComponent<PlayerBallController>().PickUpBall();
        }
    }

    public static void SetAttached(int playerID)
    {
        Hashtable someCustomPropertiesToSet = new Hashtable() { { NetworkRoomKeys.NamePlayerHoldingBall, playerID } };
        PhotonNetwork.room.SetCustomProperties(someCustomPropertiesToSet);
    }
    public static void Detach()
    {
        Hashtable someCustomPropertiesToSet = new Hashtable() { { NetworkRoomKeys.NamePlayerHoldingBall, -1 } };
        PhotonNetwork.room.SetCustomProperties(someCustomPropertiesToSet);
    }

    public static bool IsAttached()
    {
        return CustomProperties.GetProperty<int>(NetworkRoomKeys.NamePlayerHoldingBall) != -1;
    }

    public static int GetAttachedPlayerID()
    {
        object attachedPlayerID;
        PhotonNetwork.room.customProperties.TryGetValue(NetworkRoomKeys.NamePlayerHoldingBall, out attachedPlayerID);
        return (int)attachedPlayerID;
    }

    public static GameObject GetAttachedPlayer()
    {
        foreach (GameObject player in Tags.FindPlayers())
        {
            if (player.GetPhotonView().viewID == GetAttachedPlayerID())
                return player;
        }
        return null;
    }

}
