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

    void Awake()
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

    public override void ReceiveNetworkMessage(ConnectionId id, NetworkMessage message)
    {
        switch (message.type)
        {
            case MessageType.ViewPacket:
                //Debug.Log("SubId : " + message.subId + "   " + observedComponents[message.subId]);
                observedComponents[message.subId].PacketReceived(id, message.data);
                break;
            case MessageType.RPC:
                RPCCallReceived(message);
                break;
            default:
                throw new UnhandledSwitchCaseException(message.type);
        }
    }

    public void SendData(int observedId, MessageType type, byte[] data)
    {
        MyGameObjects.NetworkManagement.SendData(viewId, observedId, type, data);
    }

    public void SendData(int observedId, MessageType type, byte[] data, ConnectionId id)
    {
        MyGameObjects.NetworkManagement.SendData(viewId, observedId, type, data, id);
    }

    public void RPC(string methodName, RPCTargets targets, params object[] parameters)
    {
        Debug.Log("Call RPC : " + methodName);
        NetworkMessage message = new NetworkMessage(viewId, 0, targets, new RPCCall(methodName, parameters).Serialize());
        MyGameObjects.NetworkManagement.SendData(message);
        if (targets.IsInvokedInPlace())
        {
            RPCCallReceived(message);
        }
    }

    protected void RPCCallReceived(NetworkMessage message)
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
        if (methodParameters.Length < callParameterTypes.Length)
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

    public static GameObject Instantiate(string path, Vector3 position, Quaternion rotation)
    {
        GameObject prefabGo;

        prefabGo = (GameObject)Resources.Load(path, typeof(GameObject));

        if (prefabGo == null)
        {
            Debug.LogError("Failed to Instantiate prefab: " + path + ". Verify the Prefab is in a Resources folder (and not in a subfolder)");
            return null;
        }

        // a scene object instantiated with network visibility has to contain a NetworkView
        if (prefabGo.GetComponent<MyNetworkView>() == null)
        {
            Debug.LogError("Failed to Instantiate prefab:" + path + ". Prefab must have a NetworkView component.");
            return null;
        }
        GameObject go = (GameObject)GameObject.Instantiate(prefabGo, position, rotation);
        MyNetworkView view = go.GetComponent<MyNetworkView>();
        view.viewId = nextViewId;
        view.isMine = true;
        InstantiationMessage content = new InstantiationMessage(view.viewId, path, position, rotation);
        nextViewId++;

        MyGameObjects.NetworkManagement.SendData(MyGameObjects.NetworkManagement.viewId, 0, MessageType.Instantiate, content.Serialize());
        return go;
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

[Serializable]
public struct InstantiationMessage
{
    public int newViewId;
    public string path;
    public Vector3 position;
    public Quaternion rotation;

    public InstantiationMessage(int id, string path, Vector3 position, Quaternion rotation)
    {
        this.newViewId = id;
        this.path = path;
        this.position = position;
        this.rotation = rotation;
    }
}
