using UnityEngine;
using UnityEngine.Assertions;

public class RemovePhysicsIfClient : SlideBall.MonoBehaviour
{
    public bool DestroyFullObject;

    void Start()
    {
        if (MyComponents.NetworkManagement.IsConnected && !NetworkingState.IsServer)
            DestroyPhysics();
        else if (!MyComponents.NetworkManagement.IsConnected)
            ((NetworkManagement)MyComponents.NetworkManagement).ConnectedToRoom += DestroyPhysics;
    }

    void DestroyPhysics()
    {
        Assert.IsTrue(!NetworkingState.IsServer);
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

    private void OnDestroy()
    {
        try
        {
            ((NetworkManagement)MyComponents.NetworkManagement).ConnectedToRoom -= DestroyPhysics;
        }
        catch
        {
            //Nothing
        }
    }
}
