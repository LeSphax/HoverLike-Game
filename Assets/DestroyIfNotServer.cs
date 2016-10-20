using UnityEngine;
using System.Collections;

public class DestroyIfNotServer : MonoBehaviour
{
    void Start()
    {
        if (!MyGameObjects.NetworkManagement.isServer)
            Destroy(gameObject);
    }
}
