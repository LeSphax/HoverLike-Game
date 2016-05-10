using System;
using ExitGames.Client.Photon;
using UnityEngine;

public class BallState : Photon.MonoBehaviour
{

    private static GameObject ball;
    private static bool catchDetectorEnabled = true;
    private static float timeLastAttached;
    private const float NO_TAKING_DURATION = 0.5f;

    void Start()
    {
        ball = gameObject;
        photonView.ownershipTransfer = OwnershipOption.Takeover;
        if (!PhotonNetwork.room.customProperties.ContainsKey(NetworkRoomKeys.IsAttachedBall))
        {
            Detach();
        }
        if (GetAttachedPlayerID() != -1)
        {
            GetAttachedPlayer().GetComponent<PlayerBallController>().AttachBall();
        }
        if (!photonView.isMine)
        {
            foreach (Transform child in ball.transform)
            {
                if (child.tag == Tags.CatchDetector)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

    public static void SetAttached(int playerID)
    {
        Hashtable someCustomPropertiesToSet = new Hashtable() { { NetworkRoomKeys.IsAttachedBall, true }, { NetworkRoomKeys.NamePlayerHoldingBall, playerID } };
        PhotonNetwork.room.SetCustomProperties(someCustomPropertiesToSet);
        EnableCatchDetector(false);
        timeLastAttached = Time.realtimeSinceStartup;
      //  GetAttachedPlayer().GetPhotonView().RPC("RequestControlOfTheBall", PhotonTargets.All);
    }

    internal static bool IsTakeable()
    {
        return Time.realtimeSinceStartup - timeLastAttached > NO_TAKING_DURATION;
    }

    public static void Detach()
    {
        Hashtable someCustomPropertiesToSet = new Hashtable() { { NetworkRoomKeys.IsAttachedBall, false }, { NetworkRoomKeys.NamePlayerHoldingBall, -1 } };
        PhotonNetwork.room.SetCustomProperties(someCustomPropertiesToSet);
        EnableCatchDetector(true);
    }

    private static void EnableCatchDetector(bool v)
    {
        catchDetectorEnabled = v;
    }

    internal static bool IsCatchDetectorEnabled()
    {
        return catchDetectorEnabled;
    }

    public static bool IsAttached()
    {
        object isAttached;
        PhotonNetwork.room.customProperties.TryGetValue(NetworkRoomKeys.IsAttachedBall, out isAttached);
        return (bool)isAttached;
    }

    public static int GetAttachedPlayerID()
    {
        object attachedPlayerID;
        PhotonNetwork.room.customProperties.TryGetValue(NetworkRoomKeys.NamePlayerHoldingBall, out attachedPlayerID);
        return (int)attachedPlayerID;
    }

    public static GameObject GetAttachedPlayer()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag(Tags.Player))
        {
            if (player.GetPhotonView().viewID == GetAttachedPlayerID())
                return player;
        }
        return null;
    }

}
