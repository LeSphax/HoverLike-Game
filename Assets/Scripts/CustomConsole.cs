using UnityEngine;
using System.Collections;
using Byn.Common;
using System.Text;
using System;

public class CustomConsole : MonoBehaviour
{
    public bool debug;
    public bool log;
    // Use this for initialization
    void Start()
    {
        if (debug)
        {
            DebugHelper.ActivateConsole();
        }
        if (log)
            SLog.SetLogger(OnLog);

        SLog.LV("Verbose log is active!");
        SLog.LD("Debug mode is active");
    }

    private void OnLog(object msg, string[] tags)
    {
        StringBuilder builder = new StringBuilder();
        TimeSpan time = DateTime.Now - DateTime.Today;
        builder.Append(time);
        builder.Append("[");
        for (int i = 0; i < tags.Length; i++)
        {
            if (i != 0)
                builder.Append(",");
            builder.Append(tags[i]);
        }
        builder.Append("]");
        builder.Append(msg);
        Debug.Log(builder.ToString());
    }

    private void OnGUI()
    {
        //draws the debug console (or the show button in the corner to open it)
        DebugHelper.DrawConsole();
    }
}
