using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System;
using Byn.Net;
using System.Collections.Generic;
using Byn.Common;
using UnityEngine.Assertions;
using PlayerManagement;

public delegate void NetworkEventHandler(byte[] data, ConnectionId id);
public delegate void ConnectionEventHandler(ConnectionId id);

public class NetworkManagement : SlideBall.MonoBehaviour
{
    /* 
 * Copyright (C) 2015 Christoph Kutza
 * 
 * Please refer to the LICENSE file for license information
 */



    private const string HEROKU_URL = "ws://sphaxtest.herokuapp.com";
    private const string BCS_URL = "wss://because-why-not.com:12777/chatapp";

    private const string LOCALHOST_URL = "ws://localhost:5000";

    public enum Server
    {
        LOCALHOST,
        HEROKU,
        BCS,
    }

    public Server server;

    public string uSignalingUrl
    {
        get
        {
            switch (server)
            {
                case Server.LOCALHOST:
                    return LOCALHOST_URL;
                case Server.HEROKU:
                    return HEROKU_URL;
                case Server.BCS:
                    return BCS_URL;
                default:
                    throw new UnhandledSwitchCaseException(server);
            }
        }
    }

    private enum State
    {
        IDLE,
        CONNECTED,
        SERVER
    }

    private State stateCurrent = State.IDLE;

    public bool uLog;

    [HideInInspector]
    public string RoomName = null;

    /// <summary>
    /// Mozilla stun server. Used to get trough the firewall and establish direct connections.
    /// Replace this with your own production server as well. 
    /// </summary>
    public string uStunServer = "stun:stun.l.google.com:19302";

    /// <summary>
    /// The network interface.
    /// This can be native webrtc or the browser webrtc version.
    /// (Can also be the old or new unity network but this isn't part of this package)
    /// </summary>
    private IBasicNetwork mNetwork = null;

    /// <summary>
    /// True if the user opened an own room allowing incoming connections
    /// </summary>
    [NonSerialized]
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
    private const string GET_ROOMS_COMMAND = "___GetRooms";
    private List<NetworkMessage> bufferedMessages = new List<NetworkMessage>();

    public event EmptyEventHandler RoomCreated;
    public event ConnectionEventHandler NewPlayerConnectedToRoom;
    public event EmptyEventHandler ConnectedToRoom;
    public event EmptyEventHandler ServerStartFailed;

    public event EmptyEventHandler ReceivedAllBufferedMessages;

    /// <summary>
    /// Will setup webrtc and create the network object
    /// </summary>
	private void Start()
    {
        if (uLog)
            SLog.SetLogger(OnLog);

        SLog.LV("Verbose log is active!");
        SLog.LD("Debug mode is active");

        Debug.Log("Setting up WebRtcNetworkFactory");
        WebRtcNetworkFactory factory = WebRtcNetworkFactory.Instance;
        if (factory != null)
            Debug.Log("WebRtcNetworkFactory created");

        //InvokeRepeating("KeepConnectionAlive");
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
        Debug.Log("Initializing webrtc network");
        mNetwork = WebRtcNetworkFactory.Instance.CreateDefault(uSignalingUrl, new string[] { uStunServer });
        if (mNetwork != null)
        {
            Debug.Log("WebRTCNetwork created");
        }
        else
        {
            Debug.Log("Failed to access webrtc ");
        }
    }

    public void GetRooms()
    {
        Setup();
        mNetwork.Connect(GET_ROOMS_COMMAND);
    }

    private void Reset()
    {
        Debug.Log("Cleanup!");

        isServer = false;
        mConnections = new List<ConnectionId>();
        Cleanup();
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
    private void FixedUpdate()
    {
        HandleNetwork();
    }
    private void HandleNetwork()
    {
        //check if the network was created
        if (mNetwork != null)
        {
            //first update it to read the data from the underlaying network system
            mNetwork.Update();

            //handle all new events that happened since the last updateef
            NetworkEvent evt;

            //check for new messages and keep checking if mNetwork is available. it might get destroyed
            //due to an event
            int x = 0;
            while (mNetwork != null && mNetwork.Dequeue(out evt) && x < 100)
            {
                x++;
                switch (evt.Type)
                {
                    case NetEventType.ServerInitialized:
                        {
                            //server initialized message received
                            isServer = true;
                            Debug.LogError("Server started. Address: " + RoomName + "   " + evt.ConnectionId.id);
                            if (stateCurrent == State.IDLE)
                            {
                                SetConnectionId(ConnectionId.INVALID);
                                RoomCreated.Invoke();
                            }
                            stateCurrent = State.SERVER;
                        }
                        break;
                    //user tried to start the server but it failed
                    //maybe the user is offline or signaling server down?
                    case NetEventType.ServerInitFailed:
                        {
                            if (evt.RawData != null)
                            {
                                string rawData = (string)evt.RawData;
                                string[] rooms = rawData.Split('@');
                                if (rooms[0] == GET_ROOMS_COMMAND || rawData == GET_ROOMS_COMMAND)
                                {
                                    if (rooms.Length == 1)
                                        MyGameObjects.LobbyManager.UpdateRoomList(new string[0]);
                                    else
                                        MyGameObjects.LobbyManager.UpdateRoomList(rooms.SubArray(1, rooms.Length - 1));
                                }
                                else
                                {
                                    isServer = false;
                                    Debug.LogError("Server start failed. " + evt.RawData);
                                    if (ServerStartFailed != null)
                                    {
                                        ServerStartFailed.Invoke();
                                    }
                                    Reset();
                                    Setup();
                                }
                            }
                            else
                            {
                                Debug.LogError("No internet connection ");
                            }
                        }
                        break;
                    //server shut down. reaction to "Shutdown" call or
                    //StopServer or the connection broke down
                    case NetEventType.ServerClosed:
                        {
                            switch (stateCurrent)
                            {
                                case State.CONNECTED:
                                    ConnectToRoom(RoomName);
                                    Debug.LogError("Server closed. No incoming connections possible until restart.");
                                    break;
                                case State.IDLE:
                                    Debug.LogError("Didn't manage to create the server " + RoomName + " retrying ...");
                                    CreateRoom(RoomName);
                                    break;
                                case State.SERVER:
                                    Debug.LogError("Server closed. Restarting server");
                                    CreateRoom(RoomName);
                                    break;
                            }
                        }
                        break;
                    //either user runs a client and connected to a server or the
                    //user runs the server and a new client connected
                    case NetEventType.NewConnection:
                        {
                            Debug.LogError("NewConnection " + evt.Info + "  " + evt.ConnectionId.id);
                            mConnections.Add(evt.ConnectionId);

                            if (isServer)
                            {
                                View.RPC("SetConnectionId", evt.ConnectionId, evt.ConnectionId);
                                NewPlayerConnectedToRoom.Invoke(evt.ConnectionId);
                                SendBufferedMessages(evt.ConnectionId);
                            }
                            else if (ConnectedToRoom != null)
                            {
                                stateCurrent = State.CONNECTED;
                                ConnectedToRoom.Invoke();
                            }
                        }
                        break;
                    case NetEventType.ConnectionFailed:
                        {
                            //Outgoing connection failed. Inform the user.
                            Debug.Log("Connection failed");
                            //Reset();
                        }
                        break;
                    case NetEventType.Disconnected:
                        {
                            mConnections.Remove(evt.ConnectionId);
                            //A connection was disconnected
                            //If this was the client then he was disconnected from the server
                            //if it was the server this just means that one of the clients left
                            Debug.Log("Local Connection ID " + evt.ConnectionId + " disconnected");
                            if (isServer == false)
                            {
                                Reset();
                            }
                            else
                            {
                                string userLeftMsg = "User " + evt.ConnectionId + " left the room.";

                                //show the server the message
                                Debug.Log(userLeftMsg);
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
            if (x > 100)
            {
                Debug.LogError("More than 100 messages in buffer");
            }

            //finish this update by flushing the messages out if the network wasn't destroyed during update
            if (mNetwork != null)
                mNetwork.Flush();
        }
    }

    private void HandleIncommingMessage(ref NetworkEvent evt)
    {
        MessageDataBuffer buffer = evt.MessageData;
        NetworkMessage message = NetworkExtensions.Deserialize<NetworkMessage>(buffer.Buffer);
        if (message.traceMessage)
        {
            Debug.LogError("HandleTracedMessage " + message);
        }
        //if (message.type != MessageType.ViewPacket)
        //    Debug.LogError("Received Message : " + message.type + "   " + message.viewId);
        if (isServer && message.isDistributed())
        {
            foreach (ConnectionId id in mConnections)
            {
                if (id != evt.ConnectionId)
                {
                    SendData(message, id);
                }
            }
        }
        TryAddBuffered(message);

        MyGameObjects.NetworkViewsManagement.DistributeMessage(evt.ConnectionId, message);

        //This line of code is causing random crashes. 
        //It seems the crash occur when the time between creating and disposing is too short
        // buffer.Dispose();




    }

    public void SendData(int viewId, MessageType type, byte[] data)
    {
        NetworkMessage message = new NetworkMessage(viewId, 0, type, data);
        SendData(message, mConnections.ToArray());
    }

    public void SendData(int viewId, MessageType type, byte[] data, ConnectionId id)
    {
        NetworkMessage message = new NetworkMessage(viewId, 0, type, data);
        message.flags |= MessageFlags.NotDistributed;
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
        message.flags |= MessageFlags.NotDistributed;
        SendData(message, id);

    }

    public void SendData(NetworkMessage message)
    {
        SendData(message, mConnections.ToArray());
    }

    public void SendData(NetworkMessage message, params ConnectionId[] connectionIds)
    {
        byte[] dataToSend = message.Serialize();
        if (message.traceMessage)
        {
            Debug.LogError("SendData " + message);
        }
        for (int i = 0; i < connectionIds.Length; i++)
        {
            SendToNetwork(dataToSend, connectionIds[i], message.isReliable());
        }

        TryAddBuffered(message);
    }



    private void SendToNetwork(byte[] data, ConnectionId id, bool reliable)
    {
        mNetwork.SendData(id, data, 0, data.Length, reliable);
        if (!mConnections.Contains(id))
            Debug.LogError("This isn't a valid connectionId");
    }


    public void ConnectToRoom(string roomName)
    {
        Setup();
        RoomName = roomName;
        mNetwork.Connect(roomName);
        Debug.LogError("Connecting to " + roomName + " ...");
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
        RoomName = roomName;

        Debug.Log("StartServer " + roomName);
    }

    #region host
    public int GetNumberPlayers()
    {
        Assert.IsTrue(isServer);
        return mConnections.Count + 1;
    }

    public void SendBufferedMessages(ConnectionId id)
    {
        Assert.IsTrue(isServer);
        foreach (NetworkMessage message in bufferedMessages)
        {
            SendData(message, id);
        }
        View.RPC("AllBufferedMessagesSent", id, null);
        Debug.Log("All buffered messages sent");
    }

    private void TryAddBuffered(NetworkMessage message)
    {
        if (isServer && message.isBuffered())
        {
            message.flags = message.flags & ~MessageFlags.Buffered;
            Buffer(message);
        }
    }

    public void Buffer(NetworkMessage message)
    {
        Assert.IsTrue(isServer);
        bufferedMessages.Add(message);
    }
    #endregion

    #region client
    [MyRPC]
    private void AllBufferedMessagesSent()
    {
        ReceivedAllBufferedMessages.Invoke();
    }

    [MyRPC]
    private void SetConnectionId(ConnectionId id)
    {
        Players.myPlayerId = id;
        Players.MyPlayer.Nickname = NickNamePanel.nickname;
    }


    #endregion
}
