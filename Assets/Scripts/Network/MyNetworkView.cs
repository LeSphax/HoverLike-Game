using Byn.Net;
using SlideBall.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

//Must be added to a gameObject to allow communication with the network. All scripts on the gameObjects will be able to call RPCs.
//Moreover, ObservedComponent can be added via the inspector. These components will be able to send packets to their counterparts on the network every FixedUpdate.
public class MyNetworkView : ANetworkView
{
    RPCManager rpcManager;

    public List<ObservedComponent> observedComponents = new List<ObservedComponent>();

    [NonSerialized]
    //The view is owned by this peer.
    public bool isMine;

    //There are observedComponents that need updating
    public bool update = true;

    protected void Awake()
    {
        rpcManager = gameObject.AddComponent<RPCManager>();
        for (short i = 0; i < observedComponents.Count; i++)
        {
            observedComponents[i].observedId = i;
        }
    }

    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < observedComponents.Count; i++)
        {
            observedComponents[i].StartUpdating();
        }
    }

    void FixedUpdate()
    {
        if (update)
            foreach (ObservedComponent component in observedComponents)
            {
                if (isMine)
                {
                    component.OwnerUpdate();
                }
                else
                {
                    component.SimulationUpdate();
                }
            }
    }

    public override void ReceiveNetworkMessage(ConnectionId id, NetworkMessage message)
    {
        switch (message.type)
        {
            case MessageType.ViewPacket:
                observedComponents[message.subId].PacketReceived(id, message.data);
                break;
            case MessageType.RPC:
                rpcManager.RPCCallReceived(message, id);
                break;
            default:
                throw new UnhandledSwitchCaseException(message.type);
        }
    }

    public void SendData(short observedId, MessageType type, byte[] data)
    {
        MyComponents.NetworkManagement.SendData(ViewId, observedId, type, data);
    }

    public void SendData(short observedId, MessageType type, byte[] data, ConnectionId id)
    {
        MyComponents.NetworkManagement.SendData(ViewId, observedId, type, data, id);
    }

    public void SendData(short observedId, MessageType type, byte[] data, MessageFlags flags)
    {
        NetworkMessage message = new NetworkMessage(ViewId, observedId, type, data);
        message.flags = flags;
        MyComponents.NetworkManagement.SendData(message);
    }

    public void RPC(string methodName, ConnectionId id, params object[] parameters)
    {
        rpcManager.RPC(methodName, id, parameters);
    }

    public void RPC(string methodName, RPCTargets targets, params object[] parameters)
    {
        rpcManager.RPC(methodName, targets, parameters);
    }

    public void RPC(string methodName, RPCTargets targets, MessageFlags additionalFlags, params object[] parameters)
    {
        rpcManager.RPC(methodName, targets, additionalFlags, parameters);
    }
}
