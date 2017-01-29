/*
   Interesting links
   https://gist.github.com/stramit/c98b992c43f7313084ac
   https://gist.github.com/flarb/052467190b84657f10d2
   http://forum.unity3d.com/threads/custom-cursor-how-can-i-simulate-mouse-clicks.268513/
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

// Attach this script on your canvas's EventSystem game object
// For while it glitches the StandaloneInputModule if both are active at the same time

public class FakeCursor : BaseInputModule
{

    // Cursor object is some 3D-object inside of the canvas
    // It moves only on X and Y axis using a common player movement script
    public GameObject cursorObject = null;

    // The same event system used on the Canvas
    public EventSystem m_eventSystem = null;
    //private GameObject targetObject = null;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        if (cursorObject == null || m_eventSystem == null)
        {
            Debug.LogError("Set the game objects in the cursor module.");
            GameObject.Destroy(gameObject);
        }
    }

    protected virtual void Update()
    {
        cursorObject.transform.position = cursorObject.GetComponent<RectTransform>().position + 10f * Vector3.right * (Input.GetAxis("Mouse X"));
        cursorObject.transform.position = cursorObject.GetComponent<RectTransform>().position + 10f * Vector3.up * (Input.GetAxis("Mouse Y"));
    }

    // Process is called once per tick
    public override void Process()
    {
        // Converting the 3D-coords to Canvas-coords (it is giving wrong results, how to do this??)
        Vector3 screenPos = Camera.main.WorldToScreenPoint(cursorObject.transform.position);
        List<RaycastResult> rayResults = new List<RaycastResult>();

        float scaleFactorX = 1280.0f / Screen.width;
        float scaleFactorY = 720.0f / Screen.height;
        //float scaleFactorX = canvasScaler.referenceResolution.x / Screen.width;
        //float scaleFactorY = canvasScaler.referenceResolution.y / Screen.height;

        Vector2 rectPosition = new Vector2(screenPos.x * scaleFactorX, screenPos.y * scaleFactorY);

        // Raycasting
        PointerEventData pointer = new PointerEventData(m_eventSystem);
        pointer.position = rectPosition;
        m_eventSystem.RaycastAll(pointer, rayResults);

        if (rayResults.Count > 0)
        {
            for (int i = 0; i < rayResults.Count; i++)
            {
                Debug.Log(rayResults[i].gameObject.name);
            }
        }
    }
}
