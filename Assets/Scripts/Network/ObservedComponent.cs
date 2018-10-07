using Byn.Net;
using SlideBall.Networking;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MyNetworkView))]
public abstract class ObservedComponent : SlideBall.NetworkMonoBehaviour
{

    public static float LastBatchTime
    {
        get;
        set;
    }


    [NonSerialized]
    public short observedId;

    public int sendRate = 60;

    private static List<NetworkMessage> messagesBatch = new List<NetworkMessage>();

    private float shouldSendFloat;

    public static void SendBatch(ANetworkManagement networkManagement, NetworkViewsManagement networkViewsManagement)
    {
        if (messagesBatch.Count > 0)
        {
            byte[] data = BitConverter.GetBytes(TimeSimulation.TimeInSeconds);
            for (int i = 0; i < messagesBatch.Count; i++)
            {
                // Debug.Log(data.Length + "   " + messagesBatch[i].data.Length);
                byte[] serializedMessage = ArrayExtensions.Concatenate(BitConverter.GetBytes((short)messagesBatch[i].data.Length), messagesBatch[i].Serialize());
                data = ArrayExtensions.Concatenate(data, serializedMessage);
            }
            networkManagement.SendNetworkMessage(new NetworkMessage(networkViewsManagement.View.ViewId, MessageType.PacketBatch, data));
            messagesBatch.Clear();
        }
    }

    public virtual void PreparePacket()
    {
        if (IsSendingPackets())
        {
            shouldSendFloat += sendRate;
            // Debug.LogError(gameObject.name + "   " + shouldSendFloat + "    " + 1.0f / Time.fixedDeltaTime);
            if (sendRate == -1 || shouldSendFloat >= 1.0f / Time.deltaTime)
            {
                byte[] packet = CreatePacket();
                if (packet != null)
                {
                    shouldSendFloat -= 1.0f / Time.deltaTime;
                    MessageFlags flags;
                    if (SetFlags(out flags))
                    {
                        SendData(MessageType.ViewPacket, packet, flags);
                    }
                    else
                        SendData(MessageType.ViewPacket, packet);
                }
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
                    byte[] packet = CreatePacket();
                    if (packet != null)
                    {
                        MessageFlags flags;
                        if (SetFlags(out flags))
                        {
                            SendData(MessageType.ViewPacket, packet, flags);
                        }
                        else
                            SendData(MessageType.ViewPacket, packet);
                    }
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

    //Returns null if no changes 
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
            NetworkMessage message = new NetworkMessage(View.ViewId, observedId, type, data)
            {
                flags = flags
            };
            messagesBatch.Add(message);
        }
        else
            View.SendData(observedId, type, data, flags);
    }

    // True if flags should be set, false otherwise
    protected virtual bool SetFlags(out MessageFlags flags)
    {
        flags = MessageFlags.None;
        return false;
    }


}

