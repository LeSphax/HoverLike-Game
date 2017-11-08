using Byn.Net;
using SlideBall.Networking;
using System;
using System.Collections.Generic;
using UnityEngine;

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

    private static bool init = false;

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
        if (!init)
        {
            init = true;
        }
    }

    public bool TryGetRPCName(short methodId, out string name)
    {
        return rpcManager.TryGetRPCName(methodId, out name);
    }


    //It is pointless to send the state several times per frame (See how FixedUpdate works). So we send it only once per frame after all the FixedUpdates are done.
    //This is called first in the update execution order
    private void Update()
    {
        if (update)
            foreach (ObservedComponent component in observedComponents)
            {
                component.PreparePacket();
                if(!isMine)
                {
                    component.SimulationUpdate();
                }
            }
    }

    //We need to update the simulation on the server at a fixed rate
    void FixedUpdate()
    {
        if (update)
            foreach (ObservedComponent component in observedComponents)
            {
                if (isMine)
                {
                    component.OwnerUpdate();
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
        MyComponents.NetworkManagement.SendNetworkMessage(message);
    }

    public void RPC(string methodName, ConnectionId id, params object[] parameters)
    {
        rpcManager.RPC(methodName, id, parameters);
    }

    public void RPC(string methodName, RPCTargets targets, params object[] parameters)
    {
        //Assert.IsFalse(targets.IsInvokedInPlace() && !MyComponents.NetworkManagement.isServer && !methodName.Contains("Manual"),methodName + "    "+ targets);
        rpcManager.RPC(methodName, targets, parameters);
    }

    public void RPC(string methodName, MessageFlags additionalFlags, RPCTargets targets, params object[] parameters)
    {
        //Assert.IsFalse(targets.IsInvokedInPlace() && !MyComponents.NetworkManagement.isServer, methodName + "    " + targets);
        rpcManager.RPC(methodName, additionalFlags, targets, parameters);
    }
}
