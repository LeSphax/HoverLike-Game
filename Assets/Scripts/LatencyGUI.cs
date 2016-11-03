using UnityEngine;
using System.Collections;
using Byn.Net;
using UnityEngine.UI;
using System.Collections.Generic;

public class LatencyGUI : MonoBehaviour {

    private float currentYPosition = 0;
    public GameObject textPrefab;

    private Dictionary<ConnectionId, Text> textfields = new Dictionary<ConnectionId, Text>();

    void Awake()
    {
        MyComponents.TimeManagement.LatencyChanged += LatencyChanged;
        //MyComponents.TimeManagement.NewLatency += NewLatency;
    }

    public void LatencyChanged(ConnectionId id,float latency)
    {
        Text textfield;
        if (!textfields.TryGetValue(id, out textfield))
        {
            GameObject text = this.InstantiateAsChild(textPrefab);
            textfield = text.GetComponent<Text>();
            textfields.Add(id, textfield);
            currentYPosition -= 50;
            textfield.rectTransform.localPosition += new Vector3(0, 1, 0) * currentYPosition;
        }
        textfields[id].text = id + " : " + latency + " ms";
    }

    //public void NewLatency(ConnectionId id)
    //{
    //    Text textfield;
    //    if (!textfields.TryGetValue(id, out textfield))
    //    {
    //        GameObject text = this.InstantiateAsChild(textPrefab);
    //        textfield = text.GetComponent<Text>();
    //        textfields.Add(id, textfield);
    //        currentYPosition -= 50;
    //        textfield.rectTransform.localPosition += new Vector3(0, 1, 0) * currentYPosition;
    //    }
    //    MyComponents.TimeManagement.AddLatencyChangeListener(id, (latency) => { textfields[id].text = id + " : " + latency + " ms"; });
    //}
}
