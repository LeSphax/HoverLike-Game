using Byn.Net;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class RemovePhysicsIfClient : MonoBehaviour
{
    public bool DestroyFullObject;

    void Awake()
    {
        if (MyComponents.NetworkManagement.IsConnected && !MyComponents.NetworkManagement.isServer)
            DestroyPhysics();
        else if (!MyComponents.NetworkManagement.IsConnected)
            MyComponents.NetworkManagement.ConnectedToRoom += DestroyPhysics;
    }

    void DestroyPhysics()
    {
        Assert.IsTrue(!MyComponents.NetworkManagement.isServer);
        if (DestroyFullObject)
            Destroy(gameObject);
        else
        {
            foreach (var component in GetComponents<Collider>())
            {
                Destroy(component);
            }
            foreach (var component in GetComponents<Rigidbody>())
            {
                Destroy(component);
            }
        }
    }
}
