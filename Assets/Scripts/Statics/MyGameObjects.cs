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

    private static GameObject world;
    public static GameObject World
    {
        get
        {
            if (world == null)
            {
                world = GameObject.FindGameObjectWithTag(Tags.World);
            }
            return world;
        }
    }

    private static NetworkProperties properties;
    public static NetworkProperties Properties
    {
        get
        {
            if (properties == null)
            {
                properties = GameObject.FindGameObjectWithTag(Tags.NetworkScripts).GetComponent<NetworkProperties>();
            }
            return properties;
        }
    }

    private static MatchMaker matchMaker;
    public static MatchMaker MatchMaker
    {
        get
        {
            if (matchMaker == null)
            {
                matchMaker = GameObject.FindGameObjectWithTag(Tags.NetworkScripts).GetComponent<MatchMaker>();
            }
            return matchMaker;
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


