using UnityEngine;

public class DestroyIfNotServer : MonoBehaviour
{
    void Start()
    {
        if (!MyComponents.NetworkManagement.IsServer)
            Destroy(gameObject);
    }
}
