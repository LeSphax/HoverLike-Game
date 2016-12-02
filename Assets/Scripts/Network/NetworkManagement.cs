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
using UnityEngine.SceneManagement;
using Navigation;

public delegate void NetworkEventHandler(byte[] data, ConnectionId id);
public delegate void ConnectionEventHandler(ConnectionId id);

namespace BaseNetwork
{
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

        public bool addLatency;
        public int NumberFramesLatency;
        private LatencySimulation latencySimulation;

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

        private State _stateCurrent;
        private State stateCurrent
        {
            get
            {
                return _stateCurrent;
            }
            set
            {
                _stateCurrent = value;
                switch (value)
                {
                    case State.IDLE:
                        bufferedMessages = null;
                        if (!Scenes.IsCurrentScene(Scenes.LobbyIndex))
                            NavigationManager.LoadScene(Scenes.Lobby);
                        else
                            MyComponents.LobbyManager.Reset();
                        break;
                    case State.CONNECTED:
                        bufferedMessages = null;
                        break;
                    case State.SERVER:
                        bufferedMessages = new BufferedMessages(this);
                        break;
                    default:
                        break;
                }
            }
        }

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
        public bool isServer
        {
            get
            {
                return _stateCurrent == State.SERVER;
            }
        }

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
        private const string BLOCK_ROOMS_COMMAND = "___BlockRoom";
        private const char SPLIT_CHAR = '@';
        private BufferedMessages bufferedMessages;

        public event EmptyEventHandler RoomCreated;
        public event ConnectionEventHandler NewPlayerConnectedToRoom;
        public event EmptyEventHandler ConnectedToRoom;
        public event EmptyEventHandler ServerStartFailed;

        public event EmptyEventHandler ReceivedAllBufferedMessages;

        private void Awake()
        {
            Reset();
        }

        /// <summary>
        /// Will setup webrtc and create the network object
        /// </summary>
        private void Start()
        {
            WebRtcNetworkFactory factory = WebRtcNetworkFactory.Instance;
            if (factory != null)
                Debug.Log("WebRtcNetworkFactory created");
        }

        private void Setup()
        {
            mNetwork = WebRtcNetworkFactory.Instance.CreateDefault(uSignalingUrl, new string[] { uStunServer });
            if (mNetwork != null)
            {
                if (addLatency)
                    latencySimulation = new LatencySimulation(mNetwork, NumberFramesLatency);
            }
            else
            {
                Debug.LogError("Failed to access webrtc ");
            }
        }

        public void GetRooms()
        {
            Setup();
            mNetwork.Connect(GET_ROOMS_COMMAND);
        }

        public void BlockRoom()
        {
            Assert.IsTrue(isServer);
            mNetwork.Connect(BLOCK_ROOMS_COMMAND + SPLIT_CHAR + RoomName);
        }

        public void Reset()
        {
            Players.NewPlayerCreated -= SendBufferedMessagesOnSceneChange;
            stateCurrent = State.IDLE;
            mConnections = new List<ConnectionId>();
            Cleanup();
        }

        private void Cleanup()
        {
            if (mNetwork != null)
            {
                mNetwork.Dispose();
                mNetwork = null;
            }
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
                if (addLatency)
                    latencySimulation.NewFrame();
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
                                Debug.LogError("Server started. Address: " + RoomName + "   " + evt.ConnectionId.id);
                                if (stateCurrent == State.IDLE)
                                {
                                    stateCurrent = State.SERVER;
                                    Players.NewPlayerCreated += SendBufferedMessagesOnSceneChange;
                                    SetConnectionId(ConnectionId.INVALID);
                                    RoomCreated.Invoke();
                                }
                            }
                            break;
                        //user tried to start the server but it failed
                        //maybe the user is offline or signaling server down?
                        case NetEventType.ServerInitFailed:
                            {
                                Debug.LogError("Server Init Failed " + evt.RawData);
                                
                                if (evt.RawData != null)
                                {
                                    string rawData = (string)evt.RawData;
                                    if (rawData == NetEventMessage.ROOM_ALREADY_EXISTS)
                                    {
                                        if (ServerStartFailed != null)
                                        {
                                            Reset();
                                            Setup();
                                            ServerStartFailed.Invoke();
                                        }
                                        else
                                        {
                                            MyComponents.PopUp.Show(Language.Instance.texts["Room_Exists"] + "\n What about " + Random_Name_Generator.GetRandomName() + "?");
                                        }
                                    }
                                   
                                    string[] rooms = rawData.Split(SPLIT_CHAR);
                                    if (rooms[0] == GET_ROOMS_COMMAND || rawData == GET_ROOMS_COMMAND)
                                    {
                                        if (rooms.Length == 1)
                                            MyComponents.LobbyManager.UpdateRoomList(new string[0]);
                                        else
                                            MyComponents.LobbyManager.UpdateRoomList(rooms.SubArray(1, rooms.Length - 1));
                                    }
                                }
                                else
                                {
                                    MyComponents.PopUp.Show(Language.Instance.texts["Failed_Connect"] + "\n " + Language.Instance.texts["Feedback"]);
                                    Debug.LogError("No internet connection ");
                                }
                            }
                            break;
                        //server shut down. reaction to "Shutdown" call or
                        //StopServer or the connection broke down
                        case NetEventType.ServerClosed:
                            {
                                Reset();
                                switch (stateCurrent)
                                {
                                    case State.CONNECTED:
                                        ConnectToRoom(RoomName);
                                        Debug.LogError("Server closed. No incoming connections possible until restart.");
                                        break;
                                    case State.IDLE:
                                        MyComponents.PopUp.Show("It was not possible to create the server." + "\n" + Language.Instance.texts["Feedback"]);
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
                                Debug.LogError("Connection failed ");
                                if (evt.RawData != null)
                                {
                                    string rawdata = (string)evt.RawData;
                                    if (rawdata == NetEventMessage.ROOM_DOESNT_EXIST)
                                    {
                                        MyComponents.PopUp.Show(Language.Instance.texts["Doesnt_Exist"]);
                                    }
                                    else if (rawdata == NetEventMessage.ROOM_BLOCKED)
                                    {
                                        MyComponents.PopUp.Show(Language.Instance.texts["Room_Blocked"]);
                                    }
                                    else if (rawdata == NetEventMessage.SERVER_CONNECTION_NOT_1)
                                    {
                                        MyComponents.PopUp.Show("Server Connection is not 1");
                                    }
                                    MyComponents.LobbyManager.RefreshServers();
                                }
                            }
                            break;
                        case NetEventType.Disconnected:
                            {
                                mConnections.Remove(evt.ConnectionId);
                                //A connection was disconnected
                                //If this was the client then he was disconnected from the server
                                //if it was the server this just means that one of the clients left
                                Debug.LogError("Local Connection ID " + evt.ConnectionId + " disconnected");
                                if (isServer == false)
                                {
                                    MyComponents.ResetNetworkComponents();
                                    MyComponents.PopUp.Show(Language.Instance.texts["Client_Disconnected"]);
                                }
                                else
                                {
                                    mConnections.Remove(evt.ConnectionId);
                                    Players.Remove(evt.ConnectionId);
                                    Debug.LogError("User " + evt.ConnectionId + " left the room.");
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

        private void SendBufferedMessagesOnSceneChange(ConnectionId id)
        {
            Players.players[id].SceneChanged += (connectionId, sceneId) => { bufferedMessages.SendBufferedMessages(connectionId, sceneId); };
        }

        private void HandleIncommingMessage(ref NetworkEvent evt)
        {
            MessageDataBuffer buffer = evt.MessageData;
            NetworkMessage message = NetworkExtensions.Deserialize<NetworkMessage>(buffer.Buffer);
            if (message.traceMessage)
            {
                Debug.LogError("HandleTracedMessage " + message);
            }
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
            if (isServer)
                bufferedMessages.TryAddBuffered(message);

            MyComponents.NetworkViewsManagement.DistributeMessage(evt.ConnectionId, message);

            //This line of code is causing random crashes. 
            //It seems the crash occur when the time between creating and disposing is too short
            // buffer.Dispose();
        }

        public void SendData(short viewId, MessageType type, byte[] data)
        {
            NetworkMessage message = new NetworkMessage(viewId, 0, type, data);
            SendData(message, mConnections.ToArray());
        }

        public void SendData(short viewId, MessageType type, byte[] data, ConnectionId id)
        {
            NetworkMessage message = new NetworkMessage(viewId, 0, type, data);
            message.flags |= MessageFlags.NotDistributed;
            SendData(message, id);
        }

        public void SendData(short viewId, short subId, MessageType type, byte[] data)
        {
            NetworkMessage message = new NetworkMessage(viewId, subId, type, data);
            SendData(message, mConnections.ToArray());
        }

        public void SendData(short viewId, short subId, MessageType type, byte[] data, ConnectionId id)
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
            if (isServer)
                bufferedMessages.TryAddBuffered(message);
        }



        private void SendToNetwork(byte[] data, ConnectionId id, bool reliable)
        {
            if (addLatency)
                latencySimulation.AddMessage(data, id, reliable);
            else
                mNetwork.SendData(id, data, 0, data.Length, reliable);
            if (!mConnections.Contains(id))
                Debug.LogError("This isn't a valid connectionId");
        }


        public void ConnectToRoom(string roomName)
        {
            Setup();
            RoomName = roomName;
            mNetwork.Connect(roomName);
            MyComponents.PopUp.Show("Connecting to Room ...");
            ConnectedToRoom += ClosePopUp;
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
            MyComponents.PopUp.Show("Connecting to server ...");
            RoomCreated += ClosePopUp;
            Debug.LogError("StartServer " + roomName);
        }

        private void ClosePopUp()
        {
            RoomCreated -= ClosePopUp;
            ConnectedToRoom -= ClosePopUp;
            MyComponents.PopUp.ClosePopUp();
        }

        #region host
        public int GetNumberPlayers()
        {
            Assert.IsTrue(isServer);
            return mConnections.Count + 1;
        }
        #endregion

        #region client
        [MyRPC]
        private void ReceivedAllBuffered()
        {
            if (ReceivedAllBufferedMessages != null)
                ReceivedAllBufferedMessages.Invoke();
        }

        [MyRPC]
        private void SetConnectionId(ConnectionId id)
        {
            Players.CreatePlayer(id);
            Players.myPlayerId = id;
            Players.MyPlayer.Nickname = JavascriptAPI.nickname;
            Players.MyPlayer.SceneId = Scenes.currentSceneId;
            NavigationManager.FinishedLoadingScene += () => { Players.MyPlayer.SceneId = Scenes.currentSceneId; };
        }

        #endregion
    }
}
