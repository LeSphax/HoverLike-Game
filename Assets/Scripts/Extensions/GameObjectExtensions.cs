using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{

    public static MyNetworkView GetNetworkView(this GameObject go)
    {
        return go.GetComponent<MyNetworkView>();
    }
}
