using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    public string key;

    private static Dictionary<string, DontDestroyOnLoad> instances = new Dictionary<string, DontDestroyOnLoad>();

    protected void Awake()
    {
        if (instances.ContainsKey(key))
        {
            Destroy(this);
            gameObject.SetActive(false);
        }
        else
        {
            DontDestroyOnLoad(this);
            instances.Add(key, this);
        }
    }


}
