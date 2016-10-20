using Byn.Net;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class NetworkViewsManagement : SlideBall.MonoBehaviour
{
    private Dictionary<int, ANetworkView> networkViews = new Dictionary<int, ANetworkView>();
    private const short INSTANCIATION_INTERVAL = 10;

    public event EmptyEventHandler ReadyToInstantiate;

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

    private short nextClientViewId = -1;

    void Awake()
    {
        MyGameObjects.NetworkManagement.NewPlayerConnectedToRoom += SendClientInstanciationInterval;
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
        if (ReadyToInstantiate != null)
            ReadyToInstantiate.Invoke();
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
        MyNetworkView newView = MonoBehaviourExtensions.InstantiateFromMessage(this, message).GetComponent<MyNetworkView>();
        networkViews.Add(message.newViewId, newView);
        newView.ViewId = message.newViewId;
        newView.registered = true;
        newView.isMine = false;

        InitializeNewObject(message.initialisationParameters, newView.gameObject);
    }

    private static void InitializeNewObject(object[] initialisationParameters, GameObject go)
    {
        if (initialisationParameters != null && initialisationParameters.Length > 0)
            go.SendMessage("InitView", initialisationParameters);
    }

    internal void DistributeMessage(ConnectionId connectionId, NetworkMessage message)
    {
        if (networkViews.ContainsKey(message.viewId))
            networkViews[message.viewId].ReceiveNetworkMessage(connectionId, message);
        else
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
