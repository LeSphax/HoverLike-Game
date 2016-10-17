using UnityEngine;

public static class NetworkExtensions
{

    public static byte[] Serialize<T>(this T myObject)
    {
        return Functions.ObjectToByteArray(myObject);
    }

    public static T Deserialize<T>(byte[] data)
    {
        return (T)Functions.ByteArrayToObject(data);
    }

}
