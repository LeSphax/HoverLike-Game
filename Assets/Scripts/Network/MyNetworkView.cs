using Byn.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class MyNetworkView : ANetworkView
{
    private Dictionary<string, RPCHandler> rpcs = new Dictionary<string, RPCHandler>();

    public List<ObservedComponent> observedComponents = new List<ObservedComponent>();

    [NonSerialized]
    public bool isMine;

    public bool update = true;

    protected void Awake()
    {
        foreach (var component in GetComponents<Component>())
        {
            var methods = component.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public
        | BindingFlags.Instance | BindingFlags.Static)
                          .Where(m => m.GetCustomAttributes(typeof(MyRPC), false).Length > 0)
                          .ToArray();
            for (int i = 0; i < methods.Length; i++)
            {
                Assert.IsFalse(rpcs.ContainsKey(methods[i].Name));
                rpcs.Add(methods[i].Name, new RPCHandler(component, methods[i]));
            }
        }
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
        //Debug.Log(ViewId + "    " +  message);

        switch (message.type)
        {
            case MessageType.ViewPacket:
                observedComponents[message.subId].PacketReceived(id, message.data);
                break;
            case MessageType.RPC:
                RPCCallReceived(message, id);
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
        Debug.Log(this + "Call RPC : " + methodName + " on Id : " + id);
        if (id == ConnectionId.INVALID)
            RPC(methodName, RPCTargets.Server, parameters);
        else
        {
            NetworkMessage message = new NetworkMessage(ViewId, 0, RPCTargets.Specified, new RPCCall(methodName, parameters).Serialize());
            MyComponents.NetworkManagement.SendData(message, id);
        }
    }

    public void RPC(string methodName, RPCTargets targets, params object[] parameters)
    {
        Debug.Log(this + " : Call RPC : " + methodName);
        NetworkMessage message = new NetworkMessage(ViewId, 0, targets, new RPCCall(methodName, parameters).Serialize());
        //
        if (targets.IsSent())
            MyComponents.NetworkManagement.SendData(message);
        //
        if (targets.IsInvokedInPlace())
        {
            RPCCallReceived(message, ConnectionId.INVALID);
        }
    }

    protected void RPCCallReceived(NetworkMessage message, ConnectionId connectionId)
    {
        RPCCall call = NetworkExtensions.Deserialize<RPCCall>(message.data);
        if (call.parameters == null)
        {
            call.parameters = new object[0];
        }
        Debug.Log(this + ": RPCCallReceived " + call.methodName);
        RPCHandler handler;
        if (rpcs.TryGetValue(call.methodName, out handler))
        {
            Type[] argTypes = new Type[0];
            if (call.parameters.Length > 0)
            {
                argTypes = new Type[call.parameters.Length];
                int i = 0;
                for (int index = 0; index < call.parameters.Length; index++)
                {
                    object objX = call.parameters[index];
                    if (objX == null)
                    {
                        argTypes[i] = null;
                    }
                    else
                    {
                        argTypes[i] = objX.GetType();
                    }

                    i++;
                }
            }
            if (CheckTypeMatch(handler.methodInfo.GetParameters(), argTypes))
            {
                if (HasSenderIdParameter(handler.methodInfo.GetParameters(), argTypes))
                    handler.Invoke(ArrayExtensions.Concatenate(call.parameters, new object[1] { connectionId }));
                else
                    handler.Invoke(call.parameters);
            }
        }
        else
        {
            Debug.LogError("The method " + call.methodName + " isn't present in the list of RPCs");
        }
    }

    private bool CheckTypeMatch(ParameterInfo[] localParameters, Type[] callParameters)
    {
        if (localParameters.Length < callParameters.Length || (localParameters.Length > callParameters.Length && !HasSenderIdParameter(localParameters, callParameters)))
        {
            Debug.LogError("RPCCallReceived but the arguments length don't match : (Local)" + localParameters.Length + " vs (Received)" + callParameters.Length);
            return false;
        }

        for (int index = 0; index < callParameters.Length; index++)
        {
#if NETFX_CORE
            TypeInfo methodParamTI = methodParameters[index].ParameterType.GetTypeInfo();
            TypeInfo callParamTI = callParameterTypes[index].GetTypeInfo();

            if (callParameterTypes[index] != null && !methodParamTI.IsAssignableFrom(callParamTI) && !(callParamTI.IsEnum && System.Enum.GetUnderlyingType(methodParamTI.AsType()).GetTypeInfo().IsAssignableFrom(callParamTI)))
            {
                Debug.LogError("RPCCallReceived but the arguments types don't match : (Received)" + methodParamTI + " vs (Local)" + callParamTI);
                return false;
            }
#else
            Type type = localParameters[index].ParameterType;
            if (callParameters[index] != null && !type.IsAssignableFrom(callParameters[index]) && !(type.IsEnum && System.Enum.GetUnderlyingType(type).IsAssignableFrom(callParameters[index])))
            {
                Debug.LogError("RPCCallReceived but the arguments types don't match : (Received)" + type + " vs (Local)" + callParameters[index]);
                return false;
            }
#endif
        }

        return true;
    }

    private bool HasSenderIdParameter(ParameterInfo[] methodParameters, Type[] callParameterTypes)
    {
        //Debug.Log((methodParameters.Length == 0) + "  " + (callParameterTypes.Length == (methodParameters.Length - 1)) + "   " + (methodParameters[methodParameters.Length - 1].Name == "RPCSenderId") + " " + (methodParameters[methodParameters.Length - 1].ParameterType != typeof(ConnectionId)));
        if (methodParameters.Length == 0)
            return false;
        return (callParameterTypes.Length == methodParameters.Length - 1 && methodParameters[methodParameters.Length - 1].Name == "RPCSenderId" && methodParameters[methodParameters.Length - 1].ParameterType == typeof(ConnectionId));
    }

    private struct RPCHandler
    {
        public Component component;
        public MethodInfo methodInfo;

        public RPCHandler(Component component, MethodInfo methodInfo) : this()
        {
            this.component = component;
            this.methodInfo = methodInfo;
        }

        internal void Invoke(object[] parameters)
        {
            methodInfo.Invoke(component, parameters);
        }
    }
}
