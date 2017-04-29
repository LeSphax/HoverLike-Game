using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextInit : MonoBehaviour {

    public string key;

	// Use this for initialization
	void Start () {
        GetComponentInChildren<Text>().text = Language.Instance.texts[key];
	}
}
