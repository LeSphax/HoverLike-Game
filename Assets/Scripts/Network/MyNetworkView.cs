using Byn.Net;
using SlideBall.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;


public class MyNetworkView : ANetworkView
{
    private Dictionary<short, RPCHandler> idsToRPC = new Dictionary<short, RPCHandler>();
    private Dictionary<string, short> methodNamesToIds = new Dictionary<string, short>();


    public List<ObservedComponent> observedComponents = new List<ObservedComponent>();

    [NonSerialized]
    public bool isMine;

    public bool update = true;

    protected void Awake()
    {
        short currentId = 0;
        foreach (var component in GetComponents<Component>())
        {
            var methods = component.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public
        | BindingFlags.Instance | BindingFlags.Static)
                          .Where(m => m.GetCustomAttributes(typeof(MyRPC), false).Length > 0)
                          .ToArray();
            Dictionary<string, RPCHandler> dico = new Dictionary<string, RPCHandler>();
            for (int i = 0; i < methods.Length; i++)
            {
                Assert.IsFalse(dico.ContainsKey(methods[i].Name));
                dico.Add(methods[i].Name, new RPCHandler(component, methods[i]));
            }
            List<string> list = new List<string>(dico.Keys);
            for (short i = 0; i < list.Count; i++)
            {
                short realId = (short)(i + currentId);
                //Debug.Log(this + "   " + realId + list[i]);
                methodNamesToIds.Add(list[i], realId);
                idsToRPC.Add(realId, dico[list[i]]);
            }
            currentId += (short)list.Count;
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
                if (isMine || isLocal)
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
        if (id == ConnectionId.INVALID)
            RPC(methodName, RPCTargets.Server, parameters);
        else
        {
            NetworkMessage message = new NetworkMessage(ViewId, 0, RPCTargets.Specified, CreateRPCData(methodName, parameters));
            MyComponents.NetworkManagement.SendData(message, id);
        }
    }

    public void RPC(string methodName, RPCTargets targets, params object[] parameters)
    {
        NetworkMessage message = new NetworkMessage(ViewId, 0, targets, CreateRPCData(methodName, parameters));
        //
        RPC(targets, message);
    }

    public void RPC(string methodName, RPCTargets targets, MessageFlags additionalFlags, params object[] parameters)
    {
        NetworkMessage message = new NetworkMessage(ViewId, 0, targets, CreateRPCData(methodName, parameters));
        message.flags = message.flags | additionalFlags;
        //
        RPC(targets, message);
    }

    private byte[] CreateRPCData(string methodName, object[] parameters)
    {
        short methodId = methodNamesToIds[methodName];
        byte[] idData = BitConverter.GetBytes(methodId);
        if (parameters == null)
            return idData;
        byte[] parametersData = SerializeParameters(parameters, idsToRPC[methodId].methodInfo);
        byte[] result = ArrayExtensions.Concatenate(idData, parametersData);

        return result;
    }

    private void RPC(RPCTargets targets, NetworkMessage message)
    {
        if (targets.IsSent() && !isLocal)
            MyComponents.NetworkManagement.SendData(message);
        //
        if (targets.IsInvokedInPlace() || isLocal)
        {
            RPCCallReceived(message, ConnectionId.INVALID);
        }
    }

    private byte[] SerializeParameters(object[] parameters, MethodInfo info)
    {
        byte[] result = new byte[0];
        foreach (var parameter in parameters)
        {
            Serializer serializer;
            if (NetworkExtensions.Serializers.TryGetValue(parameter.GetType(), out serializer))
            {

            }
            else if (typeof(Array).IsAssignableFrom(parameter.GetType()))
            {
                serializer = NetworkExtensions.ArraySerializer;
            }
            else
            {
                Debug.LogError("There is no serializer for this type " + parameter.GetType());
                return result;
            }
            byte[] parameterData = serializer(parameter);
            result = ArrayExtensions.Concatenate(result, parameterData);
        }
        return result;
    }

    private List<object> DeserializeParameters(byte[] data, MethodInfo info, ref int currentIndex)
    {
        List<object> result = new List<object>();
        foreach (var parameterInfo in info.GetParameters())
        {
            if (!IsSenderIdParameter(parameterInfo))
            {
                Deserializer deserializer;
                if (NetworkExtensions.Deserializers.TryGetValue(parameterInfo.ParameterType, out deserializer))
                {
                    object parameter = deserializer(data, ref currentIndex);
                    result.Add(parameter);
                }
                else if (typeof(Array).IsAssignableFrom(parameterInfo.ParameterType))
                {
                    object parameter = NetworkExtensions.ArrayDeserializer(parameterInfo.ParameterType, data, ref currentIndex);
                    result.Add(parameter);
                }
                else
                {
                    Debug.LogError("There is no deserializer for this type " + parameterInfo.ParameterType);
                }
            }
        }
        return result;
    }

    protected void RPCCallReceived(NetworkMessage message, ConnectionId connectionId)
    {
        int currentIndex = 0;
        short methodId = BitConverter.ToInt16(message.data, currentIndex);
        currentIndex += 2;
        //
        RPCHandler handler;
        if (idsToRPC.TryGetValue(methodId, out handler))
        {
            List<object> parameters = DeserializeParameters(message.data, handler.methodInfo, ref currentIndex);
            Type[] argTypes = new Type[0];
            if (parameters.Count > 0)
            {
                argTypes = new Type[parameters.Count];
                int i = 0;
                for (int index = 0; index < parameters.Count; index++)
                {
                    object objX = parameters[index];
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
            var parameterInfos = handler.methodInfo.GetParameters();
            if (CheckTypeMatch(parameterInfos, argTypes))
            {

                if (HasSenderIdParameter(parameterInfos, argTypes))
                    handler.Invoke(ArrayExtensions.Concatenate(parameters.ToArray(), new object[1] { connectionId }));
                else
                    handler.Invoke(parameters.ToArray());
            }
        }
        else
        {
            string s = "";
            foreach (var rpc in idsToRPC)
            {
                s += rpc.Key + " : " + rpc.Value.methodInfo.Name + " /n";
            }
            Debug.LogError("The method " + methodId + " isn't present in the list of RPCs :\n" + s);
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
        if (methodParameters.Length == 0)
            return false;
        return callParameterTypes.Length == methodParameters.Length - 1 && IsSenderIdParameter(methodParameters[methodParameters.Length - 1]);
    }

    private static bool IsSenderIdParameter(ParameterInfo parameter)
    {
        return parameter.Name == "RPCSenderId" && parameter.ParameterType == typeof(ConnectionId);
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
