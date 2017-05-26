using UnityEngine;

public class LogDestroy : MonoBehaviour {
    private void OnDestroy()
    {
        Debug.LogError(gameObject.name + " is destroyed");
    }
}