using Byn.Net;
using Navigation;
using SlideBall.Networking;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class NetworkViewsManagement : SlideBall.MonoBehaviour
{
    private Dictionary<int, ANetworkView> networkViews = new Dictionary<int, ANetworkView>();

    private const short INSTANCIATION_INTERVAL = 100;
    private short nextClientViewId = INVALID_VIEW_ID;

    public const short INVALID_VIEW_ID = 0;
    private short nextViewId = INVALID_VIEW_ID;
    public short NextViewId
    {
        get
        {
            if (nextViewId == INVALID_VIEW_ID)
            {
                if (MyComponents.NetworkManagement.isServer)
                {
                    nextViewId = Settings.GetNextViewId();
                    nextClientViewId = (short)(nextViewId + INSTANCIATION_INTERVAL);
                }
                else
                {
                    Debug.LogError("Call to nextViewId before it was set");
                }
            }
            return nextViewId;
        }
    }

    public void IncrementNextViewId()
    {
        nextViewId++;
    }

    public void PartialReset()
    {
        nextClientViewId = INVALID_VIEW_ID;
        nextViewId = INVALID_VIEW_ID;
    }

    public void ResetViews()
    {
        foreach (var pair in networkViews)
        {
            if (pair.Value == null)
            {
                networkViews.Remove(pair.Key);
                Debug.LogError("Remove view " + pair.Key);
            }
        }
    }

    public void SendClientInstanciationInterval(ConnectionId id)
    {
        Assert.IsFalse(nextClientViewId == INVALID_VIEW_ID);
        View.RPC("SetInstanciationInterval", id, nextClientViewId);
        nextClientViewId += INSTANCIATION_INTERVAL;
    }

    [MyRPC]
    private void SetInstanciationInterval(short nextViewId)
    {
        this.nextViewId = nextViewId;
    }

    public GameObject Instantiate(string path)
    {
        return Instantiate(path, Vector3.zero, Quaternion.identity);
    }

    public void InstantiateOnServer(string path, Vector3 position, Quaternion rotation, params object[] initialisationParameters)
    {
        InstantiationMessage content = new InstantiationMessage(path, position, rotation, initialisationParameters);
        View.RPC("RemoteInstantiate", RPCTargets.Server, content);
    }

    public GameObject Instantiate(string path, Vector3 position, Quaternion rotation, params object[] initialisationParameters)
    {
        GameObject prefabGo;

        prefabGo = (GameObject)Resources.Load(path, typeof(GameObject));

        if (prefabGo == null)
        {
            Debug.LogError("Failed to Instantiate prefab: " + path + ". Verify the Prefab is in a Resources folder (and not in a subfolder)");
            return null;
        }

        GameObject go = (GameObject)GameObject.Instantiate(prefabGo, position, rotation);
        MyNetworkView newView = go.GetComponent<MyNetworkView>();
        InstantiationMessage content;
        if (newView != null)
        {
            newView.ViewId = NextViewId;
            newView.isMine = true;
            IncrementNextViewId();
            content = new InstantiationMessage(newView.ViewId, path, position, rotation, initialisationParameters);
            View.RPC("RemoteInstantiate", RPCTargets.OthersBuffered, content);
        }
        else
        {
            content = new InstantiationMessage(path, position, rotation, initialisationParameters);
            View.RPC("RemoteInstantiate", RPCTargets.Others, content);
        }
        InitializeNewObject(initialisationParameters, go);
        return go;
    }

    [MyRPC]
    private void RemoteInstantiate(InstantiationMessage message, ConnectionId RPCSenderId)
    {
        if (message.newViewId == INVALID_VIEW_ID)
        {
            GameObject go = MonoBehaviourExtensions.InstantiateFromMessage(this, message);
            InitializeNewObject(message.initialisationParameters, go);

        }
        else if (!networkViews.ContainsKey(message.newViewId))
        {
            MyNetworkView newView = MonoBehaviourExtensions.InstantiateFromMessage(this, message).GetComponent<MyNetworkView>();
            networkViews.Add(message.newViewId, newView);
            newView.ViewId = message.newViewId;
            newView.registered = true;
            newView.isMine = false;

            InitializeNewObject(message.initialisationParameters, newView.gameObject);
        }
        else
        {
            //Can happen when something
            // Debug.LogError("This id has already been taken " + message.newViewId);
        }
    }

    private static void InitializeNewObject(object[] initialisationParameters, GameObject go)
    {
        if (initialisationParameters != null && initialisationParameters.Length > 0)
            go.SendMessage("InitView", initialisationParameters);
    }

    int logCount = 0;

    internal void DistributeMessage(ConnectionId connectionId, NetworkMessage message)
    {
        if (networkViews.ContainsKey(message.viewId) && (Scenes.IsCurrentScene(message.sceneId) || !message.isSceneDependant()))
        {
            if (message.traceMessage)
                Debug.LogError("Message Sent to view " + message);
            networkViews[message.viewId].ReceiveNetworkMessage(connectionId, message);
        }
        else if (networkViews.ContainsKey(message.viewId) && message.isSceneDependant())
        {
            Debug.LogError("Message received in wrong scene " + Scenes.currentSceneId + "  vs " + message.sceneId + "   " + message);
            RPCCall call = NetworkExtensions.Deserialize<RPCCall>(message.data);
            Debug.LogError(call.methodName);
        }
        else
        {
            if (logCount < 5)
            {
                Debug.LogError("No view was registered with this Id " + message.viewId + "\n" + PrintViews());
                logCount++;
            }
        }
    }

    public void RegisterView(ANetworkView view)
    {
        if (networkViews.ContainsKey(view.ViewId))
        {
            Debug.LogError("This viewid (" + view.ViewId + ") is already used by " + networkViews[view.ViewId] + "- The view :" + view + " - View count : " + networkViews.Count);
        }
        else
        {
            networkViews.Add(view.ViewId, view);
            view.registered = true;
        }
    }

    internal void UnregisterView(ANetworkView view)
    {
        if (!networkViews.ContainsKey(view.ViewId))
        {
            Debug.LogError("This viewid (" + view.ViewId + ") wasn't registered " + networkViews.Count);
        }
        else
        {
            networkViews.Remove(view.ViewId);
            view.registered = false;
        }
    }

    public string PrintViews()
    {
        string result = "";
        foreach (var key in networkViews.Keys)
        {
            if (networkViews.ContainsKey(key))
                result += key + "-" + networkViews[key].ViewId + "   :" + networkViews[key] + "\n";
        }
        return result;
    }
}

[Serializable]
public struct InstantiationMessage
{
    public short newViewId;
    public string path;
    public Vector3 position;
    public Quaternion rotation;
    public object[] initialisationParameters;

    public InstantiationMessage(short id, string path, Vector3 position, Quaternion rotation, object[] initialisationParameters)
    {
        this.newViewId = id;
        this.path = path;
        this.position = position;
        this.rotation = rotation;
        this.initialisationParameters = initialisationParameters;
    }

    public InstantiationMessage(string path, Vector3 position, Quaternion rotation, object[] initialisationParameters)
    {
        this.newViewId = NetworkViewsManagement.INVALID_VIEW_ID;
        this.path = path;
        this.position = position;
        this.rotation = rotation;
        this.initialisationParameters = initialisationParameters;
    }
}
