using UnityEngine;

public static class ObjectExtensions
{
    public static T InstantiateRessource<T>(this UnityEngine.Object instantiator, string path) where T : UnityEngine.Object
    {
        T resource = Resources.Load<T>(path);
        return UnityEngine.Object.Instantiate<T>(resource);
    }

    public static GameObject InstantiateRessource(this UnityEngine.Object instantiator, string path, Transform parent) 
    {
        GameObject go = instantiator.InstantiateRessource<GameObject>(path);
        go.transform.SetParent(parent, false);
        return go;
    }

}
