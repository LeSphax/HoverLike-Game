using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace SlideBall.Networking
{
    [Serializable]
    public class NetworkMessage
    {
        // Encoding 3 int values into an int 32 value
        private const short MAX_NUMBER_SCENES = 256;
        private const int MAX_NUMBER_VIEWS = Int16.MaxValue + 1;
        private const short MAX_NUMBER_SUBVIEWS = 256;
        private int id;
        public short viewId
        {
            get
            {
                return (short)((id % (MAX_NUMBER_SUBVIEWS * MAX_NUMBER_VIEWS)) / MAX_NUMBER_SUBVIEWS);
            }
            set
            {
                id = sceneId * (MAX_NUMBER_SUBVIEWS * MAX_NUMBER_VIEWS) + value * MAX_NUMBER_SUBVIEWS + subId;
            }
        }
        public short subId
        {
            get
            {
                return (short)(id % MAX_NUMBER_SUBVIEWS);
            }
            set
            {
                Assert.IsFalse(value >= MAX_NUMBER_SUBVIEWS);
                id = sceneId * (MAX_NUMBER_SUBVIEWS * MAX_NUMBER_VIEWS) + viewId * MAX_NUMBER_SUBVIEWS + value;

            }
        }
        public short sceneId
        {
            get
            {
                return (short)(id / (MAX_NUMBER_SUBVIEWS * MAX_NUMBER_VIEWS));
            }
            set
            {
                Assert.IsFalse(value >= MAX_NUMBER_SCENES);
                id = value * (MAX_NUMBER_SUBVIEWS * MAX_NUMBER_VIEWS) + viewId * MAX_NUMBER_SUBVIEWS + subId;
            }
        }
        public MessageType type;
        public MessageFlags flags = MessageFlags.None;
        public byte[] data;

        public bool traceMessage = false;

        private NetworkMessage(int id, bool traceMessage, MessageType type, MessageFlags flags, byte[] data)
        {
            this.id = id;
            this.traceMessage = traceMessage;
            this.type = type;
            this.flags = flags;
            this.data = data;
        }

        protected NetworkMessage(short viewId, byte[] data)
        {
            this.sceneId = Scenes.currentSceneId;
            this.viewId = viewId;
            this.data = data;
        }

        public NetworkMessage(short viewId, MessageType type, byte[] data) : this(viewId, data)
        {
            this.type = type;
            SetFlagsFromType();
        }

        public NetworkMessage(short viewId, short subId, MessageType type, byte[] data) : this(viewId, type, data)
        {
            this.subId = subId;
        }

        public NetworkMessage(short viewId, short subId, RPCTargets targets, byte[] data) : this(viewId, data)
        {
            this.subId = subId;
            type = MessageType.RPC;
            flags |= targets.GetFlags();
        }

        private void SetFlagsFromType()
        {
            if (IsTypeBuffered())
            {
                flags |= MessageFlags.Buffered;
            }
            if (IsTypeNotDistributed())
            {
                flags |= MessageFlags.NotDistributed;
            }
            if (IsTypeReliable())
            {
                flags |= MessageFlags.Reliable;
            }
            if (IsTypeSceneDependant())
            {
                flags |= MessageFlags.SceneDependant;
            }
        }

        public bool isReliable()
        {
            return (flags & MessageFlags.Reliable) != 0;
        }

        public bool isBuffered()
        {
            return (flags & MessageFlags.Buffered) != 0;
        }

        public bool isDistributed()
        {
            return (flags & MessageFlags.NotDistributed) == 0;
        }

        public bool isSceneDependant()
        {
            return (flags & MessageFlags.SceneDependant) != 0;
        }

        public bool isSentToTeam()
        {
            return (flags & MessageFlags.SentToTeam) != 0;
        }

        private bool IsTypeReliable()
        {
            switch (type)
            {
                case MessageType.Properties:
                case MessageType.RPC:
                case MessageType.Synchronisation:
                    return true;
                case MessageType.ViewPacket:
                case MessageType.PacketBatch:
                    return false;
                default:
                    throw new UnhandledSwitchCaseException(type);
            }
        }



        private bool IsTypeBuffered()
        {
            switch (type)
            {
                case MessageType.RPC:
                case MessageType.ViewPacket:
                case MessageType.Properties:
                case MessageType.Synchronisation:
                case MessageType.PacketBatch:
                    return false;
                default:
                    throw new UnhandledSwitchCaseException(type);
            }
        }



        private bool IsTypeNotDistributed()
        {
            switch (type)
            {

                case MessageType.RPC:
                case MessageType.ViewPacket:
                case MessageType.Properties:
                case MessageType.PacketBatch:
                    return false;
                case MessageType.Synchronisation:
                    return true;
                default:
                    throw new UnhandledSwitchCaseException(type);
            }
        }

        private bool IsTypeSceneDependant()
        {
            switch (type)
            {
                case MessageType.RPC:
                case MessageType.ViewPacket:
                case MessageType.Properties:
                case MessageType.Synchronisation:
                case MessageType.PacketBatch:
                    return false;
                default:
                    throw new UnhandledSwitchCaseException(type);
            }
        }

        public byte[] Serialize()
        {
            byte[] serializedMessage = BitConverter.GetBytes(id);
            serializedMessage = ArrayExtensions.Concatenate(serializedMessage, BitConverter.GetBytes(traceMessage));
            serializedMessage = ArrayExtensions.Concatenate(serializedMessage, new byte[1] { (byte)type });
            serializedMessage = ArrayExtensions.Concatenate(serializedMessage, new byte[1] { (byte)flags });
            return ArrayExtensions.Concatenate(serializedMessage, data);
        }

        public static NetworkMessage Deserialize(byte[] data)
        {
            int currentIndex = 0;
            return Deserialize(data, ref currentIndex, data.Length - 7);
        }

        public static NetworkMessage Deserialize(byte[] data, ref int currentIndex, int dataLength)
        {
            int id = BitConverter.ToInt32(data, currentIndex);
            currentIndex += 4;
            bool traceMessage = BitConverter.ToBoolean(data, currentIndex);
            currentIndex++;
            MessageType type = (MessageType)data[currentIndex];
            currentIndex++;
            MessageFlags flags = (MessageFlags)data[currentIndex];
            currentIndex++;
            //Debug.Log(dataLength + "   " + currentIndex + "     " + data.Length);
            byte[] content = data.SubArray(currentIndex,dataLength);
            currentIndex += content.Length;
            Assert.AreEqual(dataLength, content.Length);
            return new NetworkMessage(id, traceMessage, type, flags, content);
        }

        public override string ToString()
        {
            string result = "Id : " + sceneId + "-" + viewId + "-" + subId + ", length : " + data.Length + ", type : " + type + " flags : " + flags + "  IsBuffered :" + isBuffered();
            if (type == MessageType.RPC)
            {
                ANetworkView view;
                if (MyComponents.NetworkViewsManagement.TryGetView(viewId, out view))
                {
                    MyNetworkView m_view = (MyNetworkView)view;
                    string name;
                    if (m_view.TryGetRPCName(RPCManager.GetRPCIdFromNetworkMessage(this), out name))
                        result += "  Method called " + name;
                }
            }
            return result;
        }
    }

    public enum MessageType : byte
    {
        ViewPacket,
        Properties,
        RPC,
        Synchronisation,
        PacketBatch,
    }

    [Flags]
    public enum MessageFlags : byte
    {
        None = 0,
        Reliable = 1,
        NotDistributed = 2,
        Buffered = 4,
        SceneDependant = 8,
        SentToTeam = 16,
    }
}