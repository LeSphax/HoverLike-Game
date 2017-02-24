using Byn.Net;
using Navigation;
using PlayerManagement;
using SlideBall.Networking;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public delegate void NetworkEventHandler(byte[] data, ConnectionId id);
public delegate void ConnectionEventHandler(ConnectionId id);

namespace SlideBall
{
    public class NetworkManagement : SlideBall.MonoBehaviour
    {
        /* 
     * Copyright (C) 2015 Christoph Kutza
     * 
     * Please refer to the LICENSE file for license information
     */

        public static ConnectionId SERVER_CONNECTION_ID = new ConnectionId(-1);
        public static ConnectionId INVALID_CONNECTION_ID = new ConnectionId(-100);

        private LatencySimulation latencySimulation;

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
        private IWebRtcNetwork mNetwork = null;

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
        private const string GET_ROOMS_COMMAND = "GetRooms";
        private const string BLOCK_ROOMS_COMMAND = "BlockRoom";
        private const char ROOM_SEPARATOR_CHAR = '@';
        private BufferedMessages bufferedMessages;

        public event EmptyEventHandler RoomCreated;
        public event EmptyEventHandler ConnectedToRoom;
        public event EmptyEventHandler ServerStartFailed;
        public event EmptyEventHandler ConnectionFailed;

        public event EmptyEventHandler ReceivedAllBufferedMessages;

        private void Awake()
        {
            Debug.LogError("NetworkManagement AWAKE");
            ConnectedToRoom += ClosePopUp;
            RoomCreated += ClosePopUp;
            WebRtcNetworkFactory factory = WebRtcNetworkFactory.Instance;
            if (factory != null)
                Debug.Log("WebRtcNetworkFactory created");
            Reset();
        }

        private void Setup()
        {
            mNetwork = WebRtcNetworkFactory.Instance.CreateDefault(EditorVariables.ServerURL, new string[] { uStunServer });
            if (mNetwork != null)
            {
                if (EditorVariables.AddLatency)
                    latencySimulation = new LatencySimulation(mNetwork, EditorVariables.NumberFramesLatency);
                mNetwork.ConnectToServer();
            }
            else
            {
                Debug.LogError("Failed to access webrtc ");
            }
        }

        public void SendUserCommand(string command, params string[] args)
        {
            string content = command;
            if (args != null)
                for (int i = 0; i < args.Length; i++)
                {
                    content += "@" + args[i];
                }
            mNetwork.SendSignalingEvent(Players.myPlayerId, content, NetEventType.UserCommand);
        }

        public void GetRooms()
        {
            SendUserCommand(GET_ROOMS_COMMAND);
        }

        public void BlockRoom()
        {
            Assert.IsTrue(isServer);
            SendUserCommand(BLOCK_ROOMS_COMMAND, RoomName);
        }

        public void Reset()
        {
            Debug.Log("Reset Network");
            Players.NewPlayerCreated -= SendBufferedMessagesOnSceneChange;
            stateCurrent = State.IDLE;
            mConnections = new List<ConnectionId>();
            Cleanup();
            Setup();
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
            //check if the network was created
            if (mNetwork != null)
            {
                if (EditorVariables.AddLatency)
                    latencySimulation.NewFrame();
                //first update it to read the data from the underlaying network system
                mNetwork.UpdateNetwork();

                ProcessSignalingEvents(mNetwork.SignalingEvents);
                ProcessPeerEvents(mNetwork.PeerEvents);

                //finish this update by flushing the messages out if the network wasn't destroyed during update
                if (mNetwork != null)
                    mNetwork.Flush();
            }
        }

        private void ProcessSignalingEvents(Queue<NetworkEvent> signalingEvents)
        {
            //handle all new events that happened since the last updateef
            NetworkEvent evt;

            //check for new messages and keep checking if mNetwork is available. it might get destroyed
            //due to an event
            while (signalingEvents.Count != 0)
            {
                evt = signalingEvents.Dequeue();
                //Debug.LogWarning("Network Management : New event received " + evt);
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
                    case NetEventType.ServerConnectionFailed:
                        {
                            Debug.LogError("Server Init Failed " + evt.RawData);
                            Reset();
                            MyComponents.PopUp.Show(Language.Instance.texts["Failed_Connect"] + "\n " + Language.Instance.texts["Feedback"]);
                            Debug.LogError("No internet connection ");
                        }
                        break;
                    //server shut down. reaction to "Shutdown" call or
                    //StopServer or the connection broke down
                    case NetEventType.ServerClosed:
                        {
                            switch (stateCurrent)
                            {
                                case State.CONNECTED:
                                    Reset();
                                    //ConnectToRoom(RoomName);
                                    Debug.LogError("Server closed. No incoming connections possible until restart.");
                                    break;
                                case State.IDLE:
                                    MyComponents.PopUp.Show(Language.Instance.texts["Cant_Create"] + "\n" + Language.Instance.texts["Feedback"]);
                                    Debug.LogError("Didn't manage to create the server " + RoomName);
                                    //Reset();
                                    //CreateRoom(RoomName);
                                    break;
                                case State.SERVER:
                                    Debug.LogError("Server closed. Restarting server");
                                    Reset();
                                    // CreateRoom(RoomName);
                                    break;
                            }
                        }
                        break;
                    case NetEventType.UserCommand:
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
                                    MyComponents.PopUp.Show(Language.Instance.texts["Room_Exists"] + Random_Name_Generator.GetRandomName() + "?");
                                }
                            }
                            else
                            {
                                string[] rooms = rawData.Split(ROOM_SEPARATOR_CHAR);
                                if (rooms[0] == GET_ROOMS_COMMAND || rawData == GET_ROOMS_COMMAND)
                                {
                                    MyComponents.LobbyManager.UpdateRoomList(RoomData.GetRoomData(rooms));
                                }
                            }
                        }
                        break;
                    default:
                        Debug.LogError("The signaling network shouldn't receive that type of events " + evt.Type + "   " + evt.Info + "    " + evt.MessageData);
                        break;

                }
            }
        }

        private void ProcessPeerEvents(Queue<NetworkEvent> peerEvents)
        {
            NetworkEvent evt;
            while (peerEvents.Count != 0)
            {
                evt = peerEvents.Dequeue();
                switch (evt.Type)
                {
                    //either user runs a client and connected to a server or the
                    //user runs the server and a new client connected
                    case NetEventType.NewConnection:
                        {
                            Debug.LogError("NewConnection " + evt.Info + "  " + evt.ConnectionId.id);

                            mConnections.Add(evt.ConnectionId);

                            if (isServer)
                            {
                                MyComponents.NetworkViewsManagement.SendClientInstanciationInterval(evt.ConnectionId);
                                MyComponents.Players.SendPlayersData(evt.ConnectionId);
                                View.RPC("SetConnectionId", evt.ConnectionId, evt.ConnectionId);
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
                            Debug.LogError("Connection failed " + stateCurrent);
                            if (stateCurrent == State.IDLE)
                                MyComponents.PopUp.Show(Language.Instance.texts["Connection_Failed"]);
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
                            if (ConnectionFailed != null)
                                ConnectionFailed.Invoke();
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
                            HandleIncomingEvent(ref evt);
                        }
                        break;
                    default:
                        Debug.LogError("The peer network shouldn't receive that type of events " + evt.Type + "   " + evt.Info + "    " + evt.MessageData);
                        break;
                }
            }
        }

        private void SendBufferedMessagesOnSceneChange(ConnectionId id)
        {
            Players.players[id].SceneChanged += (connectionId, sceneId) => { bufferedMessages.SendBufferedMessages(connectionId, sceneId); };
        }

        private void HandleIncomingEvent(ref NetworkEvent evt)
        {
            MessageDataBuffer buffer = evt.MessageData;
            NetworkMessage message = NetworkMessage.Deserialize(buffer.Buffer.SubArray(0, buffer.ContentLength));
            HandleIncomingMessage(evt.ConnectionId, message);
        }

        private void HandleIncomingMessage(ConnectionId senderId, NetworkMessage message)
        {
            if (message.traceMessage)
            {
                Debug.LogError("HandleTracedMessage " + message + "   RPCID :" + BitConverter.ToInt16(message.data, 0));
            }
            if (isServer && message.isDistributed())
            {
                foreach (ConnectionId id in mConnections)
                {
                    if (id != senderId)
                    {
                        SendData(message, id);
                    }
                }
            }
            if (isServer)
                bufferedMessages.TryAddBuffered(senderId, message);

            MyComponents.NetworkViewsManagement.DistributeMessage(senderId, message);

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
                bufferedMessages.TryAddBuffered(SERVER_CONNECTION_ID, message);
        }

        private void SendToNetwork(byte[] data, ConnectionId id, bool reliable)
        {
            if (EditorVariables.AddLatency)
                latencySimulation.AddMessage(data, id, reliable);
            else
                mNetwork.SendPeerEvent(id, data, 0, data.Length, reliable);
            if (!mConnections.Contains(id))
                Debug.LogError("This isn't a valid connectionId " + id + "    " + mConnections.Count + "   " + mConnections.PrintContent());
        }

        public void ConnectToRoom(string roomName)
        {
            RoomName = roomName;
            mNetwork.ConnectToRoom(roomName);
            MyComponents.PopUp.Show(Language.Instance.texts["Connecting"]);
            Debug.LogError("Connecting to room : " + roomName + " ...");
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
            EnsureLength(roomName);
            mNetwork.CreateRoom(roomName);
            RoomName = roomName;
            MyComponents.PopUp.Show(Language.Instance.texts["Connecting"]);

            Debug.LogError("CreateRoom " + roomName);
        }

        private void ClosePopUp()
        {
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
            Players.MyPlayer.Nickname = UserSettings.Nickname;
            Players.MyPlayer.SceneId = Scenes.currentSceneId;
            NavigationManager.FinishedLoadingScene += (previousSceneId, currentSceneId) => { Players.MyPlayer.SceneId = currentSceneId; };
        }

        #endregion
    }
}
