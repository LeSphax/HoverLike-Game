using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {

    [SerializeField]
    private Text content;
    public string Content
    {
        get
        {
            return content.text;
        }
        set
        {
            content.text = value;
        }
    }

    [SerializeField]
    private Text title;
    public string Title
    {
        get
        {
            return title.text;
        }
        set
        {
            title.text = value;
        }
    }
}
