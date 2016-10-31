using Byn.Net;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class NetworkViewsManagement : SlideBall.MonoBehaviour
{
    private Dictionary<int, ANetworkView> networkViews = new Dictionary<int, ANetworkView>();

    private const short INSTANCIATION_INTERVAL = 100;
    private short nextClientViewId = -1;

    private short nextViewId = -1;
    public short NextViewId
    {
        get
        {
            if (nextViewId == -1)
            {
                if (MyGameObjects.NetworkManagement.isServer)
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

    void Awake()
    {
        MyGameObjects.NetworkManagement.NewPlayerConnectedToRoom += SendClientInstanciationInterval;
    }

    public void Reset()
    {
        nextClientViewId = -1;
        nextViewId = -1;
    }

    private void SendClientInstanciationInterval(ConnectionId id)
    {
        Assert.IsFalse(nextClientViewId == -1);
        View.RPC("SetInstanciationInterval", id, nextClientViewId);
        nextClientViewId += INSTANCIATION_INTERVAL;
    }

    [MyRPC]
    private void SetInstanciationInterval(short nextViewId)
    {
        this.nextViewId = nextViewId;
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

        // a scene object instantiated with network visibility has to contain a NetworkView
        if (prefabGo.GetComponent<MyNetworkView>() == null)
        {
            Debug.LogError("Failed to Instantiate prefab:" + path + ". Prefab must have a NetworkView component.");
            return null;
        }
        GameObject go = (GameObject)GameObject.Instantiate(prefabGo, position, rotation);
        MyNetworkView newView = go.GetComponent<MyNetworkView>();
        newView.ViewId = NextViewId;
        newView.isMine = true;
        Debug.Log("Instantiate " + newView.ViewId);
        InstantiationMessage content = new InstantiationMessage(newView.ViewId, path, position, rotation, initialisationParameters);
        View.RPC("RemoteInstantiate", RPCTargets.OthersBuffered, content);
        IncrementNextViewId();
        InitializeNewObject(initialisationParameters, go);
        return go;
    }

    [MyRPC]
    private void RemoteInstantiate(InstantiationMessage message, ConnectionId RPCSenderId)
    {
        if (!networkViews.ContainsKey(message.newViewId))
        {
            MyNetworkView newView = MonoBehaviourExtensions.InstantiateFromMessage(this, message).GetComponent<MyNetworkView>();
            networkViews.Add(message.newViewId, newView);
            newView.ViewId = message.newViewId;
            newView.registered = true;
            newView.isMine = false;

            InitializeNewObject(message.initialisationParameters, newView.gameObject);
        }
    }

    private static void InitializeNewObject(object[] initialisationParameters, GameObject go)
    {
        if (initialisationParameters != null && initialisationParameters.Length > 0)
            go.SendMessage("InitView", initialisationParameters);
    }

    internal void DistributeMessage(ConnectionId connectionId, NetworkMessage message)
    {
        if (networkViews.ContainsKey(message.viewId) && (Scenes.IsCurrentScene(message.sceneId) || !message.isSceneDependant()))
            networkViews[message.viewId].ReceiveNetworkMessage(connectionId, message);
        else if (message.isSceneDependant())
        {
            Debug.LogError("Message received in wrong scene " + Scenes.currentSceneId + "  vs " + message.sceneId + "   " + message);
            RPCCall call = NetworkExtensions.Deserialize<RPCCall>(message.data);
            Debug.LogError(call.methodName);
        }
        return;// Debug.Log("No view was registered with this Id " + message.viewId);


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


    public void PrintViews()
    {
        for (int i = 0; i < networkViews.Count; i++)
        {
            if (networkViews.ContainsKey(i))
                Debug.LogError(i + "-" + networkViews[i].ViewId + "   :" + networkViews[i]);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            PrintViews();
        }
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
}
