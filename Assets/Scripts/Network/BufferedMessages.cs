using Byn.Net;
using Navigation;
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

        private Dictionary<int, List<StoredMessage>> bufferedMessages = new Dictionary<int, List<StoredMessage>>();


        public BufferedMessages(NetworkManagement networkManagement)
        {
            Assert.IsTrue(networkManagement.IsServer);
            this.networkManagement = networkManagement;
            NavigationManager.FinishedLoadingScene += SceneChanged;
        }

        internal void SendBufferedMessages(ConnectionId id, short sceneId)
        {
            //Debug.LogWarning("SendBuffered messages " + id + "   " + sceneId);
            List<StoredMessage> messages;
            if (bufferedMessages.TryGetValue(sceneId, out messages))
            {
                foreach (StoredMessage storedMessage in messages)
                {
                    if (id == NetworkManagement.SERVER_CONNECTION_ID)
                    {
                        MyComponents.NetworkViewsManagement.DistributeMessage(storedMessage.senderId, storedMessage.message);
                    }
                    else
                    {
                        networkManagement.SendNetworkMessage(storedMessage.message, id);
                    }
                }
            }
            networkManagement.View.RPC("ReceivedAllBuffered", id, null);
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
                Debug.Log("Store " + message.sceneId + "   " + Scenes.MainIndex);
            }
            list.Add(new StoredMessage(senderId, message));
        }

        private void SceneChanged(short previousSceneId, short currentSceneId)
        {
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
