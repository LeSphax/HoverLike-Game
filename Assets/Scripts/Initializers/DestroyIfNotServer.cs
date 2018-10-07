using UnityEngine;

public class DestroyIfNotServer : MonoBehaviour
{
    void Start()
    {
        if (!NetworkingState.IsServer)
            Destroy(gameObject);
    }
}
