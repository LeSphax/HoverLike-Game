using UnityEngine;
using System.Collections;

public class EditorScripts : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
#if UNITY_EDITOR
        gameObject.AddComponent<EditorCompilation>();
#endif
    }
}
