using UnityEngine;

public delegate void ConnectedToRoom();

public class Matchmaker : Photon.PunBehaviour
{
    private PhotonView myPhotonView;
    private bool creator = false;

    public static event ConnectedToRoom connectedEvent;

    // Use this for initialization
    public void Start()
    {
        PhotonNetwork.sendRate = 60;
        PhotonNetwork.sendRateOnSerialize = 60;
        PhotonNetwork.ConnectUsingSettings("0.1");
        PhotonNetwork.OverrideBestCloudServer(CloudRegionCode.eu);
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnConnectedToMaster()
    {
        // when AutoJoinLobby is off, this method gets called when PUN finished the connection (instead of OnJoinedLobby())
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnPhotonRandomJoinFailed()
    {
        PhotonNetwork.CreateRoom(null);
        creator = true;
        Debug.Log("JoinedFailed");
    }

    public override void OnJoinedRoom()
    {
        
        int numberPlayers = PhotonNetwork.playerList.Length-1;
        if (creator)
        {
            PhotonNetwork.Instantiate("Ball", new Vector3(10, 10, 10), Quaternion.identity, 0);
        }
        GameObject player = PhotonNetwork.Instantiate("MyPlayer", new Vector3(0, 4.4f, 0), Quaternion.identity, 0);
        player.GetComponent<PlayerController>().Init(numberPlayers%2, "Player" + numberPlayers);
        if (connectedEvent != null)
            connectedEvent.Invoke();
        else
            Debug.LogError("The connection event should be listened to " + connectedEvent);
    }

    public void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        GUILayout.Label(PhotonNetwork.GetPing()+"");
    }
}
