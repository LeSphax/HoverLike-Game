using Byn.Net;
using SlideBall.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class RPCManager : SlideBall.MonoBehaviour
{
    public const string SenderIdParameterName = "RPCSenderId";
    private Dictionary<short, RPCHandler> idsToRPC = new Dictionary<short, RPCHandler>();
    private Dictionary<string, short> methodNamesToIds = new Dictionary<string, short>();

    private void Awake()
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
                Assert.IsFalse(dico.ContainsKey(methods[i].Name), this + " : Two RPCs on the same GameObject shouldn't have the same name");
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
    }


    public void RPC(string methodName, ConnectionId id, params object[] parameters)
    {
        if (id == ConnectionId.INVALID)
            RPC(methodName, RPCTargets.Server, parameters);
        else
        {
            NetworkMessage message = new NetworkMessage(View.ViewId, 0, RPCTargets.Specified, CreateRPCData(methodName, parameters));
            MyComponents.NetworkManagement.SendNetworkMessage(message, id);
        }
    }

    public void RPC(string methodName, RPCTargets targets, params object[] parameters)
    {
        NetworkMessage message = new NetworkMessage(View.ViewId, 0, targets, CreateRPCData(methodName, parameters));
        //
        RPC(targets, message);
    }

    public void RPC(string methodName, MessageFlags additionalFlags, RPCTargets targets,  params object[] parameters)
    {
        NetworkMessage message = new NetworkMessage(View.ViewId, 0, targets, CreateRPCData(methodName, parameters));
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
        if (targets.IsSentToNetwork())
            MyComponents.NetworkManagement.SendNetworkMessage(message);
        //
        if (targets.IsInvokedInPlace())
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
            result = result.Concatenate(parameterData);
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

    internal void RPCCallReceived(NetworkMessage message, ConnectionId connectionId)
    {
        int currentBufferIndex = 0;
        short methodId = GetRPCIdFromNetworkMessage(message);
        currentBufferIndex += 2;
        //
        RPCHandler handler;
        if (idsToRPC.TryGetValue(methodId, out handler))
        {
            List<object> receivedParameters = DeserializeParameters(message.data, handler.methodInfo, ref currentBufferIndex);
            Type[] receivedParametersTypes = ToTypeArray(receivedParameters);

            var localParametersInfo = handler.methodInfo.GetParameters();
            if (CheckTypeMatch(localParametersInfo, receivedParametersTypes))
            {
                if (HasSenderIdParameter(localParametersInfo, receivedParametersTypes))
                    handler.Invoke(ArrayExtensions.Concatenate(receivedParameters.ToArray(), new object[1] { connectionId }));
                else
                    handler.Invoke(receivedParameters.ToArray());
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

    private static Type[] ToTypeArray(List<object> parameters)
    {
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

        return argTypes;
    }

    internal static short GetRPCIdFromNetworkMessage(NetworkMessage networkMessage)
    {
        return BitConverter.ToInt16(networkMessage.data, 0);
    }

    public bool TryGetRPCName(short methodId, out string name)
    {
        RPCHandler handler;
        name = "";
        if (idsToRPC.TryGetValue(methodId, out handler))
        {
            name = handler.methodInfo.Name;
            return true;
        }
        return false;
    }

    private bool CheckTypeMatch(ParameterInfo[] localParameters, Type[] receivedParameters)
    {
        if (localParameters.Length < receivedParameters.Length || (localParameters.Length > receivedParameters.Length && !HasSenderIdParameter(localParameters, receivedParameters)))
        {
            Debug.LogError("RPCCallReceived but the arguments length don't match : (Local)" + localParameters.Length + " vs (Received)" + receivedParameters.Length);
            return false;
        }

        for (int index = 0; index < receivedParameters.Length; index++)
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
            if (receivedParameters[index] != null && !type.IsAssignableFrom(receivedParameters[index]) && !(type.IsEnum && System.Enum.GetUnderlyingType(type).IsAssignableFrom(receivedParameters[index])))
            {
                Debug.LogError("RPCCallReceived but the arguments types don't match : (Received)" + type + " vs (Local)" + receivedParameters[index]);
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

    //The method that we call can have the id of the peer who sent the RPC as an additional parameter.
    //As the peer id is already present in NetworkMessage.connectionId, it is not replicated in the RPC parameters that are sent over the network.
    private static bool IsSenderIdParameter(ParameterInfo parameter)
    {
        return parameter.Name == SenderIdParameterName && parameter.ParameterType == typeof(ConnectionId);
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

