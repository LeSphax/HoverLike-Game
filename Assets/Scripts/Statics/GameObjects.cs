using UnityEngine;

public static class GameObjects
{
    public static GameObject UI()
    {
        return GameObject.FindGameObjectWithTag(Tags.UI);
    }

    public static Rigidbody MyPlayerRigidbody()
    {
        return GetTaggedComponent<Rigidbody>(Tags.MyPlayer);
    }

    public static GameObject MyPlayer()
    {
        return GameObject.FindGameObjectWithTag(Tags.MyPlayer);
    }

    private static Type GetTaggedComponent<Type>(string tag)
    {
        GameObject go = GameObject.FindGameObjectWithTag(tag);
        if (go != null)
            return go.GetComponent<Type>();
        return default(Type);
    }

}


