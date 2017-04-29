using UnityEngine;

public class DestroyIfNotServer : MonoBehaviour
{
    void Start()
    {
        if (!MyComponents.NetworkManagement.isServer)
            Destroy(gameObject);
    }
}
