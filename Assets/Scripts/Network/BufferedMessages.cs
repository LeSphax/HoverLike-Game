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
            Assert.IsTrue(networkManagement.isServer);
            this.networkManagement = networkManagement;
            NavigationManager.FinishedLoadingScene += SceneChanged;
        }

        internal void SendBufferedMessages(ConnectionId id, short sceneId)
        {
            Debug.Log("SendBuffered messages " + id + "   " + sceneId);
            List<StoredMessage> messages;
            if (bufferedMessages.TryGetValue(sceneId, out messages))
            {
                Debug.Log("Number buffered messages " + messages.Count);
                foreach (StoredMessage message in messages)
                {
                    if (id == NetworkManagement.SERVER_CONNECTION_ID)
                    {
                        MyComponents.NetworkViewsManagement.DistributeMessage(message.senderId, message.message);
                    }
                    else
                    {
                        networkManagement.SendData(message.message, id);
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
            }
            list.Add(new StoredMessage(senderId, message));
        }

        private void SceneChanged(short previousSceneId, short currentSceneId)
        {
            Debug.Log("Remove Messages " + previousSceneId);
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
