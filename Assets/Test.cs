using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	void OnColliderEnter(Collision collision)
    {
        Debug.LogWarning("Collision");
    }

    void OnTriggerEnter(Collider collider)
    {
        Debug.LogWarning("TCollision");
    }

    void OnPlayerEnter(object jambon)
    {
        Debug.LogWarning("PlayerEnter");
    }
}
