using Byn.Net;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class NetworkViewsManagement : SlideBall.MonoBehaviour
{
    private Dictionary<int, ANetworkView> networkViews = new Dictionary<int, ANetworkView>();

    public int NextViewId
    {
        get;
        set;
    }

    public GameObject Instantiate(string path, Vector3 position, Quaternion rotation)
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
        InstantiationMessage content = new InstantiationMessage(newView.ViewId, path, position, rotation);
        View.RPC("RemoteInstantiate", RPCTargets.OthersBuffered, content);
        NextViewId++;

        return go;
    }

    [MyRPC]
    private void RemoteInstantiate(InstantiationMessage message)
    {
        //Making sure the views will have the same id on both sides
        MyNetworkView newView = MonoBehaviourExtensions.InstantiateFromMessage(this, message).GetComponent<MyNetworkView>();
        int newViewId;
        Debug.LogError("Received Instantiate " + message.newViewId + "   " + NextViewId);
        //We don't have a view with this id yet so we can just add it
        if (!networkViews.ContainsKey(message.newViewId))
        {
            newViewId = message.newViewId;
            NextViewId = newViewId + 1;
        }
        //The new id is inferior and we already have a view with this id. This means that two instantiations happened at the same time before they were synchronised
        else if (message.newViewId < NextViewId)
        {
            // I am the server, I change the id to the next id that I would have assigned
            if (MyGameObjects.NetworkManagement.isServer)
            {
                Debug.LogWarning("Instantiation already happened, dump received id :" + message.newViewId + " -> " + NextViewId);
                newViewId = NextViewId;
                NextViewId = newViewId + 1;
            }
            //I am a client, all the ids of the views that came after mine were changed by the server
            // Shift right them and add the new view.
            else
            {
                for (int i = NextViewId; i >= message.newViewId; i--)
                {
                    if (networkViews.ContainsKey(i))
                    {
                        if (networkViews.ContainsKey(i + 1))
                            networkViews[i + 1] = networkViews[i];
                        else
                            networkViews.Add(i + 1, networkViews[i]);
                    }
                    else
                        networkViews.Remove(i + 1);
                }
                newViewId = message.newViewId;
            }
            NextViewId += 1;
        }
        else
        {
            //If the ids are equal it is normal, we can use the received view id
            //If the new id is superior, there is an instantiation message that we didn't receive yet. We can also use the new id and wait for the next message
            //If it is inferior but we don't yet have a view for this id then it is probably the instantiation message that we received late. Then we can also add it at the received id.
            newViewId = -1;
            Debug.LogError("This should never happen, we have to review our assumptions");
        }
        //Debug.Log("NetworkM - New Instantiate Id " + newViewId);
        networkViews.Add(newViewId, newView);
        newView.ViewId = newViewId;
        newView.registered = true;
        newView.isMine = false;
    }

    internal void DistributeMessage(ConnectionId connectionId, NetworkMessage message)
    {
        Assert.IsTrue(networkViews.ContainsKey(message.viewId), "No view was registered with this Id " + message.viewId);
        networkViews[message.viewId].ReceiveNetworkMessage(connectionId, message);

    }

    public void RegisterView(ANetworkView view)
    {
        if (networkViews.ContainsKey(view.ViewId))
        {
            Assert.IsFalse(networkViews.ContainsKey(view.ViewId), "This viewid is already used " + view.ViewId + "  : " + view + "  " + networkViews.Count + "   " + networkViews[view.ViewId]);
        }
        networkViews.Add(view.ViewId, view);
        view.registered = true;
        NextViewId = networkViews.Count;
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
