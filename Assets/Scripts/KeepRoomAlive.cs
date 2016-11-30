using UnityEngine;
using System.Collections;
using Byn.Net;

public class KeepRoomAlive : MonoBehaviour
{

    public string uSignalingUrl
    {
        get
        {
            return MyComponents.NetworkManagement.uSignalingUrl;
        }
    }

    /// <summary>
    /// Mozilla stun server. Used to get trough the firewall and establish direct connections.
    /// Replace this with your own production server as well. 
    /// </summary>
    public string uStunServer
    {
        get
        {
            return MyComponents.NetworkManagement.uStunServer;
        }
    }

    /// <summary>
    /// The network interface.
    /// This can be native webrtc or the browser webrtc version.
    /// (Can also be the old or new unity network but this isn't part of this package)
    /// </summary>
    private IBasicNetwork mNetwork = null;

    public string roomName
    {
        get
        {
            return MyComponents.NetworkManagement.RoomName;
        }
    }

    /// <summary>
    /// Will setup webrtc and create the network object
    /// </summary>
	private void Start()
    {
        WebRtcNetworkFactory factory = WebRtcNetworkFactory.Instance;
        if (factory == null)
            Debug.LogError("WebRtcNetworkFactory not created");

        InvokeRepeating("KeepConnectionAlive", 30, 30);
    }

    private void KeepConnectionAlive()
    {
        if (mNetwork != null)
        {
            Cleanup();
        }
        Setup();
        mNetwork.Connect(roomName);
    }

    private void Setup()
    {
        mNetwork = WebRtcNetworkFactory.Instance.CreateDefault(uSignalingUrl, new string[] { uStunServer });
        if (mNetwork == null)
        {
            Debug.LogError("Failed to access webrtc ");
        }
    }

    private void Cleanup()
    {
        mNetwork.Dispose();
        mNetwork = null;
    }

    private void OnDestroy()
    {
        if (mNetwork != null)
        {
            Cleanup();
        }
    }

}
