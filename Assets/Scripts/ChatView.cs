using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ChatView : MonoBehaviour
{

    private ScrollRect scrollRect;
    public Text text;

    private string messages;
    private StringBuilder builder;

    private float oldVerticalPosition;

    void Awake()
    {
        text.text = "";
        builder = new StringBuilder("");
        //Add enough characters to fill the content size fitter
        //It avoids having text in the middle at the beginning
        for (int i = 0; i < 20; i++)
        {
            builder.AppendLine();
        }
        SetText();

        scrollRect = GetComponent<ScrollRect>();
        
        
    }

    private void OnEnable()
    {
        MyComponents.ChatManager.NewMessage += AddMessage;
    }

    private void Start()
    {
        scrollRect.verticalNormalizedPosition = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlaceText();
        }
    }

    private void PlaceText()
    {
        if (oldVerticalPosition <= 0.1)
            scrollRect.verticalNormalizedPosition = 0;
    }

    private void AddMessage(string message)
    {
        oldVerticalPosition = scrollRect.verticalNormalizedPosition;
        builder.Append(message);
        SetText();
    }

    private void SetText()
    {
        //Show the last 1000 characters
        text.text = builder.ToString(Mathf.Max(builder.Length - 1000, 0), Mathf.Min(builder.Length-1,1000));
        Invoke("PlaceText",Time.deltaTime *2);
    }

    private void OnDisable()
    {
        MyComponents.ChatManager.NewMessage -= AddMessage;
    }
}
