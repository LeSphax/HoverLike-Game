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
        for (int i = 0; i < observedComponents.Count; i++)
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

    public void SendData(int observedId, MessageType type, byte[] data)
    {
        MyGameObjects.NetworkManagement.SendData(ViewId, observedId, type, data);
    }

    public void SendData(int observedId, MessageType type, byte[] data, ConnectionId id)
    {
        MyGameObjects.NetworkManagement.SendData(ViewId, observedId, type, data, id);
    }

    public void SendData(int observedId, MessageType type, byte[] data, MessageFlags flags)
    {
        NetworkMessage message = new NetworkMessage(ViewId, observedId, type, data);
        message.flags = flags;
        MyGameObjects.NetworkManagement.SendData(message);
    }

    public void RPC(string methodName, ConnectionId id, params object[] parameters)
    {
        Debug.Log("Call RPC : " + methodName);
        NetworkMessage message = new NetworkMessage(ViewId, 0, RPCTargets.Specified, new RPCCall(methodName, parameters).Serialize());
        MyGameObjects.NetworkManagement.SendData(message, id);
    }

    public void RPC(string methodName, RPCTargets targets, params object[] parameters)
    {
        Debug.Log("Call RPC : " + methodName);
        NetworkMessage message = new NetworkMessage(ViewId, 0, targets, new RPCCall(methodName, parameters).Serialize());
        //
        if (targets.IsSent())
            MyGameObjects.NetworkManagement.SendData(message);
        //
        if (targets.IsInvokedInPlace())
        {
            RPCCallReceived(message, ConnectionId.INVALID);
        }
    }

    protected void RPCCallReceived(NetworkMessage message, ConnectionId connectionId)
    {
        RPCCall call = NetworkExtensions.Deserialize<RPCCall>(message.data);
        Debug.Log("RPCCallReceived " + call.methodName);
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
                Debug.Log(HasSenderIdParameter(handler.methodInfo.GetParameters(), argTypes));
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

    private bool CheckTypeMatch(ParameterInfo[] methodParameters, Type[] callParameterTypes)
    {
        if (methodParameters.Length < callParameterTypes.Length || methodParameters.Length > callParameterTypes.Length && !!HasSenderIdParameter(methodParameters, callParameterTypes))
        {
            Debug.LogError("RPCCallReceived but the arguments length don't match : (Received)" + methodParameters.Length + " vs (Local)" + callParameterTypes.Length);
            return false;
        }

        for (int index = 0; index < callParameterTypes.Length; index++)
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
            Type type = methodParameters[index].ParameterType;
            if (callParameterTypes[index] != null && !type.IsAssignableFrom(callParameterTypes[index]) && !(type.IsEnum && System.Enum.GetUnderlyingType(type).IsAssignableFrom(callParameterTypes[index])))
            {
                Debug.LogError("RPCCallReceived but the arguments types don't match : (Received)" + type + " vs (Local)" + callParameterTypes[index]);
                return false;
            }
#endif
        }

        return true;
    }

    private bool HasSenderIdParameter(ParameterInfo[] methodParameters, Type[] callParameterTypes)
    {
        return (methodParameters[methodParameters.Length - 1].Name == "RPCSenderId" && methodParameters[methodParameters.Length - 1].ParameterType != typeof(ConnectionId));
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
