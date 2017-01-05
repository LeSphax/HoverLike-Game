
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class RequestParameters
{
    public static string testParameters = "http://lesphax.github.io/SlideBall/?RoomName=Jambon";
    public static string noParameters = "http://lesphax.github.io/SlideBall/";

    private static Dictionary<string, string> urlParameters = null;
    private static Dictionary<string, string> URLParameters
    {
        get
        {
            if (urlParameters == null)
            {
#if UNITY_EDITOR || !UNITY_WEBGL
                if (EditorVariables.TestURLParameters)
                    SetRequestParameters(testParameters);
                else
                    SetRequestParameters(noParameters);
#elif UNITY_WEBGL
                Debug.Log(Application.absoluteURL);
                SetRequestParameters(Application.absoluteURL);
#endif
            }
            return urlParameters;
        }
    }

    public static bool HasKey(string key)
    {
        return URLParameters.ContainsKey(key);
    }

    // This can be called from Start(), but not earlier
    public static string GetValue(string key)
    {
        return URLParameters[key];
    }

    public static void SetRequestParameters(string parametersString)
    {
        urlParameters = new Dictionary<string, string>();

        string[] split = parametersString.Split(new char[1] { '?' });
        if (split.Length > 1)
        {
            string location = split[1];

            string[] parameters = location.Split(new char[] { '&' }, System.StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < parameters.Length; ++i)
            {
                string[] keyValue = parameters[i].Split(new char[] { '=' }, System.StringSplitOptions.None);

                if (keyValue.Length >= 2)
                {
                    urlParameters.Add(WWW.UnEscapeURL(keyValue[0]), WWW.UnEscapeURL(keyValue[1]));
                }
                else if (keyValue.Length == 1)
                {
                    urlParameters.Add(WWW.UnEscapeURL(keyValue[0]), "");
                }
            }
        }
    }
}
