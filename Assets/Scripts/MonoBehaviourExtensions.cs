
using UnityEngine;


public static class MonoBehaviourExtensions
{
    public static GameObject InstantiateAsChild(this MonoBehaviour monoBehaviour, GameObject prefab)
    {
        GameObject gameObject = Object.Instantiate(prefab);
        gameObject.transform.SetParent(monoBehaviour.transform, false);
        return gameObject;
    }

    public static GameObject InstantiateFromMessage(this MonoBehaviour monoBehaviour, InstantiationMessage message)
    {
        GameObject prefab = Resources.Load<GameObject>(message.path);
        return Object.Instantiate(prefab, message.position, message.rotation);
    }
}


