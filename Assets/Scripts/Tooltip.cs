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

    [SerializeField]
    private Image icon;
    public Sprite Icon
    {
        get
        {
            return icon.sprite;
        }
        set
        {
            icon.sprite = value;
        }
    }
}
