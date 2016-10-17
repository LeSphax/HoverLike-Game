using UnityEngine;

public static class MyGameObjects
{
    private static NetworkManagement networkManagement;
    public static NetworkManagement NetworkManagement
    {
        get
        {
            if (networkManagement == null)
            {
                networkManagement = GameObject.FindGameObjectWithTag(Tags.NetworkScripts).GetComponent<NetworkManagement>();
            }
            return networkManagement;
        }
    }

    private static LobbyManager lobbyManager;
    public static LobbyManager LobbyManager
    {
        get
        {
            if (lobbyManager == null)
            {
                lobbyManager = GameObject.FindGameObjectWithTag(Tags.LobbyManager).GetComponent<LobbyManager>();
            }
            return lobbyManager;
        }
    }

    private static NetworkViewsManagement networkViewsManagement;
    public static NetworkViewsManagement NetworkViewsManagement
    {
        get
        {
            if (networkViewsManagement == null)
            {
                networkViewsManagement = GameObject.FindGameObjectWithTag(Tags.NetworkScripts).GetComponent<NetworkViewsManagement>();
            }
            return networkViewsManagement;
        }
    }

    private static GameObject room;
    public static GameObject Room
    {
        get
        {
            if (room == null)
            {
                room = GameObject.FindGameObjectWithTag(Tags.Room);
            }
            return room;
        }
    }

    private static GameObject roomActivation;
    public static GameObject RoomActivation
    {
        get
        {
            if (roomActivation == null)
            {
                roomActivation = Room.transform.FindChild("Activated").gameObject;
            }
            return roomActivation;
        }
    }

    private static GameObject scene;
    public static GameObject Scene
    {
        get
        {
            if (scene == null)
            {
                scene = GameObject.FindGameObjectWithTag(Tags.Scene);
            }
            return scene;
        }
    }

    private static NetworkProperties properties;
    public static NetworkProperties Properties
    {
        get
        {
            if (properties == null)
            {
                properties = GameObject.FindGameObjectWithTag(Tags.RoomScripts).GetComponent<NetworkProperties>();
            }
            return properties;
        }
    }

    private static RoomManager roomManager;
    public static RoomManager RoomManager
    {
        get
        {
            if (roomManager == null)
            {
                roomManager = GameObject.FindGameObjectWithTag(Tags.NetworkScripts).GetComponent<RoomManager>();
            }
            return roomManager;
        }
    }

    public static GameObject UI()
    {
        return GameObject.FindGameObjectWithTag(Tags.UI);
    }

    public static Rigidbody MyPlayerRigidbody()
    {
        return GetTaggedComponent<Rigidbody>(Tags.MyPlayer);
    }

    public static GameObject MyPlayer()
    {
        return GameObject.FindGameObjectWithTag(Tags.MyPlayer);
    }

    private static Type GetTaggedComponent<Type>(string tag)
    {
        GameObject go = GameObject.FindGameObjectWithTag(tag);
        if (go != null)
            return go.GetComponent<Type>();
        return default(Type);
    }

}


