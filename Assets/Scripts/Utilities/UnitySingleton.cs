using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitySingleton : MonoBehaviour
{
    private static UnitySingleton sInstance;

    protected void Awake()
    {
        if (sInstance == null)
        {
            sInstance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this);
    }

    public static UnitySingleton Instance
    {
        get
        {
            return sInstance;
        }
    }


}
