using UnityEngine;
using System.Collections;

public class CursorLocker : MonoBehaviour
{

    #if UNITY_WEBGL
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {

            Screen.fullScreen = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
    #endif
}
