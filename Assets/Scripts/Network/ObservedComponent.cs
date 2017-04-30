using Byn.Net;
using SlideBall.Networking;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MyNetworkView))]
public abstract class ObservedComponent : SlideBall.MonoBehaviour
{

    private static int lastReceivedBatchNumber = -1;
    public static int LastReceivedBatchNumber
    {
        get
        {
            return lastReceivedBatchNumber;
        }
        set
        {
            if (CurrentlyShownBatchNb == -1)
            {
                InitBatchNb(value - 3);
            }
            lastReceivedBatchNumber = value;
        }
    }

    public static int currentlyShownBatchNb = -1;
    public static int CurrentlyShownBatchNb
    {
        get
        {
            return currentlyShownBatchNb;
        }
        set
        {
            if (currentlyShownBatchNb != -1)
                currentlyShownBatchNb = value;
        }
    }

    [NonSerialized]
    public short observedId;

    public int sendRate = 60;

    private static List<NetworkMessage> messagesBatch = new List<NetworkMessage>();

    private float shouldSendFloat;

    public static void SendBatch()
    {
        if (messagesBatch.Count > 0)
        {
            byte[] data = BitConverter.GetBytes(CurrentlyShownBatchNb);
            for (int i = 0; i < messagesBatch.Count; i++)
            {
                // Debug.Log(data.Length + "   " + messagesBatch[i].data.Length);
                byte[] serializedMessage = ArrayExtensions.Concatenate(BitConverter.GetBytes((short)messagesBatch[i].data.Length), messagesBatch[i].Serialize());
                data = ArrayExtensions.Concatenate(data, serializedMessage);
            }
            MyComponents.NetworkManagement.SendNetworkMessage(new NetworkMessage(MyComponents.NetworkViewsManagement.View.ViewId, MessageType.PacketBatch, data));
            messagesBatch.Clear();
        }
    }

    internal static void InitBatchNb(int v)
    {
        currentlyShownBatchNb = v;
    }

    public virtual void PreparePacket()
    {
        //PerformanceTest();


        if (IsSendingPackets())
        {
            shouldSendFloat += sendRate;
            // Debug.LogError(gameObject.name + "   " + shouldSendFloat + "    " + 1.0f / Time.fixedDeltaTime);
            if (sendRate == -1 || shouldSendFloat >= 1.0f / Time.deltaTime)
            {
                shouldSendFloat -= 1.0f / Time.deltaTime;
                MessageFlags flags;
                if (SetFlags(out flags))
                {
                    SendData(MessageType.ViewPacket, CreatePacket(), flags);
                }
                else
                    SendData(MessageType.ViewPacket, CreatePacket());
            }
        }
    }

    private void PerformanceTest()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Tests for " + gameObject.name);
            var sp = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++)
            {
                if (IsSendingPackets())
                {
                    MessageFlags flags;
                    if (SetFlags(out flags))
                    {
                        SendData(MessageType.ViewPacket, CreatePacket(), flags);
                    }
                    else
                        SendData(MessageType.ViewPacket, CreatePacket());
                }
            }
            sp.Stop();
            Debug.Log("Time Send Packet " + sp.ElapsedMilliseconds);
            sp = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++)
            {
                CreatePacket();
            }
            sp.Stop();
            Debug.Log("Time Create Packet " + sp.ElapsedMilliseconds);
        }
    }

    protected abstract bool IsSendingPackets();

    public abstract void OwnerUpdate();
    public abstract void SimulationUpdate();
    public abstract bool ShouldBatchPackets();

    protected abstract byte[] CreatePacket();

    public abstract void PacketReceived(ConnectionId id, byte[] data);

    protected void SendData(MessageType type, byte[] data)
    {
        if (ShouldBatchPackets())
            messagesBatch.Add(new NetworkMessage(View.ViewId, observedId, type, data));
        else
            View.SendData(observedId, type, data);
    }
    protected void SendData(MessageType type, byte[] data, MessageFlags flags)
    {
        if (ShouldBatchPackets())
        {
            NetworkMessage message = new NetworkMessage(View.ViewId, observedId, type, data);
            message.flags = flags;
            messagesBatch.Add(message);
        }
        else
            View.SendData(observedId, type, data, flags);
    }

    // True is flags shouled be set, false otherwise
    protected virtual bool SetFlags(out MessageFlags flags)
    {
        flags = MessageFlags.None;
        return false;
    }


}

