using System;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    protected void Awake()
    {
        DontDestroyOnLoad(this);
    }


}
