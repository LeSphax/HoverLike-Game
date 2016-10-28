using Byn.Net;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace BaseNetwork
{

    public class BufferedMessages
    {
        private NetworkManagement networkManagement;

        private Dictionary<int, List<NetworkMessage>> bufferedMessages = new Dictionary<int, List<NetworkMessage>>();


        public BufferedMessages(NetworkManagement networkManagement)
        {
            Assert.IsTrue(networkManagement.isServer);
            this.networkManagement = networkManagement;
        }

        internal void SendBufferedMessages(ConnectionId id, short sceneId)
        {
            List<NetworkMessage> messages;
            if (bufferedMessages.TryGetValue(sceneId, out messages))
            {
                foreach (NetworkMessage message in messages)
                {
                    networkManagement.SendData(message, id);
                }
            }
            networkManagement.View.RPC("AllBufferedMessagesSent", id, null);
            Debug.Log("All buffered messages sent");
        }

        internal void TryAddBuffered(NetworkMessage message)
        {
            if (message.isBuffered())
            {
                message.flags = message.flags & ~MessageFlags.Buffered;
                Buffer(message);
            }
        }

        internal void Buffer(NetworkMessage message)
        {
            List<NetworkMessage> list;
            if (!bufferedMessages.TryGetValue(message.sceneId, out list))
            {
                list = new List<NetworkMessage>();
                bufferedMessages.Add(message.sceneId, list);
            }
            list.Add(message);
        }

    }
}
