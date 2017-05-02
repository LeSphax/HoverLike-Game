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
            LateFixedUpdate.evt += MyLateFixedUpdate;
        }
    }

    public bool TryGetRPCName(short methodId, out string name)
    {
        return rpcManager.TryGetRPCName(methodId, out name);
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
                component.PreparePacket();
            }
    }

    static void MyLateFixedUpdate()
    {
        if (MyComponents.NetworkManagement.isServer)
        {
            ObservedComponent.SendBatch();
            ObservedComponent.BatchNumberToSend++;
        }
        else
        {
            ObservedComponent.CurrentlyShownBatchNb++;
            if (ObservedComponent.LastReceivedBatchNumber - ObservedComponent.CurrentlyShownBatchNb > 5)
            {
                ObservedComponent.CurrentlyShownBatchNb = ObservedComponent.TargetBatchNumber;
            }
            else
            {
                ObservedComponent.CurrentlyShownBatchNb = Mathf.Lerp(ObservedComponent.CurrentlyShownBatchNb, ObservedComponent.TargetBatchNumber, Time.fixedDeltaTime * 3);
            }
        }
        //Debug.Log(ObservedComponent.CurrentlyShownBatchNb
        //    + "     " + ObservedComponent.LastReceivedBatchNumber +
        //    "    " + (ObservedComponent.LastReceivedBatchNumber - ObservedComponent.CurrentlyShownBatchNb));
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
