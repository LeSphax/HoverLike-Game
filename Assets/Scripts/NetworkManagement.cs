using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System;
using Byn.Net;
using System.Collections.Generic;
using Byn.Common;
using UnityEngine.Assertions;

public delegate void NetworkEventHandler(byte[] data, ConnectionId id);

public class NetworkManagement : ANetworkView
{
    /* 
 * Copyright (C) 2015 Christoph Kutza
 * 
 * Please refer to the LICENSE file for license information
 */



    /// <summary>
    /// This is a test server. Don't use in production! The server code is in a zip file in WebRtcNetwork
    /// </summary>
    public string uSignalingUrl = "wss://because-why-not.com:12777/chatapp";

    /// <summary>
    /// Mozilla stun server. Used to get trough the firewall and establish direct connections.
    /// Replace this with your own production server as well. 
    /// </summary>
    public string uStunServer = "stun:stun.l.google.com:19302";

    /// <summary>
    /// Set true to use send the WebRTC log + wrapper log output to the unity log.
    /// </summary>
    public bool uLog = false;

    /// <summary>
    /// Debug console to be able to see the unity log on every platform
    /// </summary>
    public bool uDebugConsole = false;

    /// <summary>
    /// The network interface.
    /// This can be native webrtc or the browser webrtc version.
    /// (Can also be the old or new unity network but this isn't part of this package)
    /// </summary>
    private IBasicNetwork mNetwork = null;

    /// <summary>
    /// True if the user opened an own room allowing incoming connections
    /// </summary>
    public bool isServer = false;

    /// <summary>
    /// Keeps track of all current connections
    /// </summary>
    private List<ConnectionId> mConnections = new List<ConnectionId>();
    public bool IsConnected
    {
        get
        {
            return mConnections.Count > 0;
        }
    }

    private const int MAX_CODE_LENGTH = 256;
    private const string RoomName = "jamboen";
    private Dictionary<int, ANetworkView> networkViews = new Dictionary<int, ANetworkView>();
    private List<NetworkMessage> bufferedMessages = new List<NetworkMessage>();

    public event EmptyEventHandler ServerCreated;
    public event EmptyEventHandler NewConnection;

    /// <summary>
    /// Will setup webrtc and create the network object
    /// </summary>
	protected override void Start()
    {
        base.Start();
        CreateRoom(RoomName);
        //shows the console on all platforms. for debugging only
        if (uDebugConsole)
            DebugHelper.ActivateConsole();
        if (uLog)
            SLog.SetLogger(OnLog);

        SLog.LV("Verbose log is active!");
        SLog.LD("Debug mode is active");

        Append("Setting up WebRtcNetworkFactory");
        WebRtcNetworkFactory factory = WebRtcNetworkFactory.Instance;
        if (factory != null)
            Append("WebRtcNetworkFactory created");

    }
    private void OnLog(object msg, string[] tags)
    {
        StringBuilder builder = new StringBuilder();
        TimeSpan time = DateTime.Now - DateTime.Today;
        builder.Append(time);
        builder.Append("[");
        for (int i = 0; i < tags.Length; i++)
        {
            if (i != 0)
                builder.Append(",");
            builder.Append(tags[i]);
        }
        builder.Append("]");
        builder.Append(msg);
        Debug.Log(builder.ToString());
    }

    private void Setup()
    {
        Append("Initializing webrtc network");
        mNetwork = WebRtcNetworkFactory.Instance.CreateDefault(uSignalingUrl, new string[] { uStunServer });
        if (mNetwork != null)
        {
            Append("WebRTCNetwork created");
        }
        else
        {
            Append("Failed to access webrtc ");
        }
    }

    private void Reset()
    {
        Debug.Log("Cleanup!");

        isServer = false;
        mConnections = new List<ConnectionId>();
        Cleanup();
    }

    /// <summary>
    /// called during reset and destroy
    /// </summary>
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

    private void FixedUpdate()
    {
        //check each fixed update if we have got new events
        HandleNetwork();
    }
    private void HandleNetwork()
    {
        //check if the network was created
        if (mNetwork != null)
        {
            //first update it to read the data from the underlaying network system
            mNetwork.Update();

            //handle all new events that happened since the last update
            NetworkEvent evt;

            //check for new messages and keep checking if mNetwork is available. it might get destroyed
            //due to an event
            while (mNetwork != null && mNetwork.Dequeue(out evt))
            {
                switch (evt.Type)
                {
                    case NetEventType.ServerInitialized:
                        {
                            //server initialized message received
                            isServer = true;
                            SetNumberPlayers();
                            string address = evt.Info;
                            Append("Server started. Address: " + address);
                            Assert.IsNotNull(ServerCreated);
                            ServerCreated.Invoke();
                        }
                        break;
                    //user tried to start the server but it failed
                    //maybe the user is offline or signaling server down?
                    case NetEventType.ServerInitFailed:
                        {

                            isServer = false;
                            Append("Server start failed.");
                            Reset();
                            Setup();
                            mNetwork.Connect(RoomName);
                        }
                        break;
                    //server shut down. reaction to "Shutdown" call or
                    //StopServer or the connection broke down
                    case NetEventType.ServerClosed:
                        {
                            isServer = false;
                            Append("Server closed. No incoming connections possible until restart.");
                        }
                        break;
                    //either user runs a client and connected to a server or the
                    //user runs the server and a new client connected
                    case NetEventType.NewConnection:
                        {
                            mConnections.Add(evt.ConnectionId);
                            SetNumberPlayers();

                            Append("New local connection! ID: " + evt.ConnectionId);

                            //if server -> send announcement to everyone and use the local id as username
                            if (isServer)
                            {
                                string msg = "New user " + evt.ConnectionId + " joined the room.";
                                Append(msg);
                            }
                            Assert.IsNotNull(NewConnection);
                            NewConnection.Invoke();
                            if (isServer)
                            {
                                SendBufferedMessages(evt.ConnectionId);
                            }

                            SendData(MyGameObjects.MatchMaker.viewId, MessageType.StartGame, null, evt.ConnectionId);
                        }
                        break;
                    case NetEventType.ConnectionFailed:
                        {
                            //Outgoing connection failed. Inform the user.
                            Append("Connection failed");
                            Reset();
                        }
                        break;
                    case NetEventType.Disconnected:
                        {
                            mConnections.Remove(evt.ConnectionId);
                            //A connection was disconnected
                            //If this was the client then he was disconnected from the server
                            //if it was the server this just means that one of the clients left
                            Append("Local Connection ID " + evt.ConnectionId + " disconnected");
                            if (isServer == false)
                            {
                                Reset();
                            }
                            else
                            {
                                string userLeftMsg = "User " + evt.ConnectionId + " left the room.";

                                //show the server the message
                                Append(userLeftMsg);
                            }
                        }
                        break;
                    case NetEventType.ReliableMessageReceived:
                    case NetEventType.UnreliableMessageReceived:
                        {
                            HandleIncommingMessage(ref evt);
                        }
                        break;
                }
            }

            //finish this update by flushing the messages out if the network wasn't destroyed during update
            if (mNetwork != null)
                mNetwork.Flush();
        }
    }

    private void SetNumberPlayers()
    {
        if (isServer)
            MyGameObjects.Properties.SetProperty(PropertiesKeys.NumberPlayers, mConnections.Count + 1);
    }

    private void SendBufferedMessages(ConnectionId id)
    {
        Assert.IsTrue(isServer);
        foreach (NetworkMessage message in bufferedMessages)
        {
            SendData(message, id);
        }
        Debug.Log("All buffered messages sent");
    }

    private void HandleIncommingMessage(ref NetworkEvent evt)
    {
        MessageDataBuffer buffer = evt.MessageData;

        NetworkMessage message = NetworkExtensions.Deserialize<NetworkMessage>(buffer.Buffer);
        if (message.type != MessageType.ViewPacket)
            Debug.Log("Received Message : " + message.type + "   " + message.viewId);
        if (isServer && message.isDistributed())
        {
            foreach (ConnectionId id in mConnections)
            {
                if (message.isSentBack() || id != evt.ConnectionId)
                    SendToNetwork(message.data, id, message.isReliable());
                break;
            }
        }

        ANetworkView handler;
        if (networkViews.TryGetValue(message.viewId, out handler))
        {
            handler.ReceiveNetworkMessage(evt.ConnectionId, message);
        }
        else
        {
            //Debug.LogError("No view was registered with this Id " + message.viewId);
            //Debug.LogError(ANetworkView.nextViewId);
            //for (int i = 0; i < 5; i++)
            //{
            //    if (networkViews.ContainsKey(i))
            //        Debug.LogError(i + "   " +networkViews[i]);
            //}
        }

        buffer.Dispose();
    }

    public override void ReceiveNetworkMessage(ConnectionId id, NetworkMessage message)
    {
        switch (message.type)
        {
            case MessageType.Instantiate:
                //Making sure the views will have the same id on both sides
                InstantiationMessage content = NetworkExtensions.Deserialize<InstantiationMessage>(message.data);
                MyNetworkView newView = MonoBehaviourExtensions.InstantiateFromMessage(this, content).GetComponent<MyNetworkView>();
                int newViewId;
                Debug.Log("Received Instantiate " + content.newViewId + "   " + nextViewId);
                //We don't have a view with this id yet so we can just add it
                if (!networkViews.ContainsKey(content.newViewId))
                {
                    newViewId = content.newViewId;
                    nextViewId = newViewId + 1;
                }
                //The new id is inferior and we already have a view with this id. This means that two instantiations happened at the same time before they were synchronised
                else if (content.newViewId < nextViewId)
                {
                    // I am the server, I change the id to the next id that I would have assigned
                    if (isServer)
                    {
                        newViewId = nextViewId;
                        nextViewId = newViewId + 1;
                    }
                    //I am a client, all the ids of the views that came after mine were changed by the server
                    // Shift right them and add the new view.
                    else
                    {
                        for (int i = nextViewId; i >= content.newViewId; i--)
                        {
                            if (networkViews.ContainsKey(i))
                            {
                                if (networkViews.ContainsKey(i + 1))
                                    networkViews[i + 1] = networkViews[i];
                                else
                                    networkViews.Add(i + 1, networkViews[i]);
                            }
                            else
                                networkViews.Remove(i + 1);
                        }
                        newViewId = content.newViewId;
                    }
                    nextViewId += 1;
                }
                else
                {
                    //If the ids are equal it is normal, we can use the received view id
                    //If the new id is superior, there is an instantiation message that we didn't receive yet. We can also use the new id and wait for the next message
                    //If it is inferior but we don't yet have a view for this id then it is probably the instantiation message that we received late. Then we can also add it at the received id.
                    newViewId = -1;
                    Debug.LogError("This should never happen, we have to review our assumptions");
                }
                Debug.Log("NetworkM - New Instantiate Id " + newViewId);
                networkViews.Add(newViewId, newView);
                newView.registered = true;
                newView.isMine = false;
                break;
            default:
                throw new UnhandledSwitchCaseException(message.type);
        }
    }

    public void SendData(int viewId, MessageType type, byte[] data)
    {
        NetworkMessage message = new NetworkMessage(viewId, 0, type, data);
        SendData(message, mConnections.ToArray());
    }

    public void SendData(int viewId, MessageType type, byte[] data, ConnectionId id)
    {
        NetworkMessage message = new NetworkMessage(viewId, 0, type, data);
        SendData(message, id);
    }

    public void SendData(int viewId, int subId, MessageType type, byte[] data)
    {
        NetworkMessage message = new NetworkMessage(viewId, subId, type, data);
        SendData(message, mConnections.ToArray());
    }

    public void SendData(int viewId, int subId, MessageType type, byte[] data, ConnectionId id)
    {
        NetworkMessage message = new NetworkMessage(viewId, subId, type, data);
        SendData(message, id);

    }

    public void SendData(NetworkMessage message, params ConnectionId[] connectionIds)
    {
        byte[] dataToSend = message.Serialize();

        foreach (ConnectionId id in connectionIds)
        {
            SendToNetwork(dataToSend, id, message.isReliable());
            break;
        }

        TryAddBuffered(message);
    }

    private void TryAddBuffered(NetworkMessage message)
    {
        if (isServer && message.isBuffered())
        {
            message.flags = message.flags & ~MessageFlags.Buffered;
            bufferedMessages.Add(message);
        }
    }

    private void SendToNetwork(byte[] data, ConnectionId id, bool reliable)
    {
        mNetwork.SendData(id, data, 0, data.Length, reliable);
    }

    public void RegisterView(ANetworkView view)
    {
        Assert.IsFalse(networkViews.ContainsKey(view.viewId), "This viewid is already used " + view.viewId + "  : " + view.gameObject.name + "  " + networkViews.Count);
        networkViews.Add(view.viewId, view);
        view.registered = true;
        nextViewId = networkViews.Count;
    }


    private void Append(string text)
    {
        Debug.Log(text);
    }

    public void ConnectToRoom(string roomName)
    {
        Setup();
        mNetwork.Connect(roomName);
        Append("Connecting to " + roomName + " ...");
    }

    private void EnsureLength(string roomName)
    {
        if (roomName.Length > MAX_CODE_LENGTH)
        {
            roomName = roomName.Substring(0, MAX_CODE_LENGTH);
        }
    }

    public void CreateRoom(string roomName)
    {
        Setup();
        EnsureLength(roomName);
        mNetwork.StartServer(roomName);

        Debug.Log("StartServer " + roomName);
    }

    public void PrintViews()
    {
        for (int i = 0; i < networkViews.Count; i++)
        {
            if (networkViews.ContainsKey(i))
                Debug.Log(i + "   :" + networkViews[i]);
        }
    }
}
