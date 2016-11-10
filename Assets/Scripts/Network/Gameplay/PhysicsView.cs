using UnityEngine;

public abstract class PhysicsView : MonoBehaviour
{

    protected void FixedUpdate()
    {
        if (MyComponents.NetworkManagement.isServer)
            ServerBehaviour();
        else
            ClientBehaviour();
    }

    protected abstract void ServerBehaviour();
    protected abstract void ClientBehaviour();
}