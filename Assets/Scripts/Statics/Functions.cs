using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

class Functions
{

    public static Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        LayerMask layerMask = (1 << 10);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            if (hit.collider.tag == Tags.Ground)
            {
                return hit.point;

            }
        return Vector3.zero;
    }

    public static byte[] ObjectToByteArray(object obj)
    {
        if (obj == null)
            return null;
        BinaryFormatter bf = new BinaryFormatter();
        bf.SurrogateSelector = Vector3SerializationSurrogate.GetSurrogateSelector();
        using (MemoryStream ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    public static object ByteArrayToObject(byte[] arrBytes)
    {
        MemoryStream memStream = new MemoryStream();
        BinaryFormatter binForm = new BinaryFormatter();
        binForm.SurrogateSelector = Vector3SerializationSurrogate.GetSurrogateSelector();

        memStream.Write(arrBytes, 0, arrBytes.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        object obj = (object)binForm.Deserialize(memStream);

        return obj;
    }
}

