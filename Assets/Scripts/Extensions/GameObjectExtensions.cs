using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{

    public static MyNetworkView GetNetworkView(this GameObject go)
    {
        return go.GetComponent<MyNetworkView>();
    }

    public static GameObject FindGameObjectWithTag(this Transform parent, string tag)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.tag == tag)
            {
                return child.gameObject;
            }
            if (child.childCount > 0)
            {
                var component = FindGameObjectWithTag(child, tag);
                if (component != null)
                    return component;
            }
        }
        return null;
    }
}
