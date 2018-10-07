using Byn.Net;
using Navigation;
using PlayerManagement;
using SlideBall.Networking;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace SlideBall.Networking
{

    public class BufferedMessages
    {
        private NetworkManagement networkManagement;
        private NetworkViewsManagement viewsManagement;

        private Dictionary<int, List<StoredMessage>> bufferedMessages = new Dictionary<int, List<StoredMessage>>();


        public BufferedMessages(NetworkManagement networkManagement, NetworkViewsManagement viewsManagement)
        {
            Assert.IsTrue(NetworkingState.IsServer);
            this.networkManagement = networkManagement;
            this.viewsManagement = viewsManagement;
            NavigationManager.FinishedLoadingScene += SceneChanged;
        }

        internal void SendBufferedMessages(Player player)
        {
            SendBufferedMessages(player.sceneId, player.id);
        }

        internal void SendBufferedMessages(short sceneId, ConnectionId connectionId)
        {
            List<StoredMessage> messages;
            if (bufferedMessages.TryGetValue(sceneId, out messages))
            {
                Debug.LogWarning("SendBuffered the " + messages.Count + "messages");
                foreach (StoredMessage storedMessage in messages)
                {
                    if (connectionId == NetworkManagement.SERVER_CONNECTION_ID)
                    {
                        viewsManagement.DistributeMessage(storedMessage.senderId, storedMessage.message);
                    }
                    else
                    {
                        networkManagement.SendNetworkMessage(storedMessage.message, connectionId);
                    }
                }
            }
            networkManagement.View.RPC("ReceivedAllBuffered", connectionId, null);
        }

        internal void TryAddBuffered(ConnectionId senderId, NetworkMessage message)
        {
            if (message.isBuffered())
            {
                message.flags = message.flags & ~MessageFlags.Buffered;
                Buffer(senderId, message);
            }
        }

        internal void Buffer(ConnectionId senderId, NetworkMessage message)
        {
            List<StoredMessage> list;
            if (!bufferedMessages.TryGetValue(message.sceneId, out list))
            {
                list = new List<StoredMessage>();
                bufferedMessages.Add(message.sceneId, list);
            }
            list.Add(new StoredMessage(senderId, message));
        }

        private void SceneChanged(short previousSceneId, short currentSceneId)
        {
            Debug.Log("Remove buffered messages for scene " + previousSceneId);
            bufferedMessages.Remove(previousSceneId);
        }

    }
}

public class StoredMessage
{
    public NetworkMessage message;
    public ConnectionId senderId;

    public StoredMessage(ConnectionId senderId, NetworkMessage message)
    {
        this.senderId = senderId;
        this.message = message;
    }
}
