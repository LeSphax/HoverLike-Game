using Byn.Net;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Utilities;

public delegate object Deserializer(byte[] data, ref int currentIndex);
public delegate byte[] Serializer(object parameter);

public static class NetworkExtensions
{
    public static Vector3 ToVector3(byte[] data, ref int currentIndex)
    {
        float x = BitConverter.ToSingle(data, currentIndex);
        currentIndex += 4;
        float y = BitConverter.ToSingle(data, currentIndex);
        currentIndex += 4;
        float z = BitConverter.ToSingle(data, currentIndex);
        currentIndex += 4;
        return new Vector3(x, y, z);
    }

    public static Map<short, Type> typeIds;
    public static Map<short, Type> TypeIds
    {
        get
        {
            if (typeIds == null)
            {
                typeIds = new Map<short, Type>();
                typeIds.Add(0, typeof(char));
                typeIds.Add(1, typeof(string));
                typeIds.Add(2, typeof(bool));
                typeIds.Add(3, typeof(float));
                typeIds.Add(4, typeof(int));
                typeIds.Add(5, typeof(short));
                typeIds.Add(6, typeof(ConnectionId));
                typeIds.Add(7, typeof(Vector3));
                typeIds.Add(8, typeof(Quaternion));
                typeIds.Add(9, typeof(InstantiationMessage));
                typeIds.Add(10, typeof(Team));
                typeIds.Add(11, typeof(MatchManager.State));
                typeIds.Add(12, typeof(Vector2));
                typeIds.Add(13, typeof(Color));
            }
            return typeIds;
        }
    }

    public static Dictionary<Type, Deserializer> deserializers;
    public static Dictionary<Type, Deserializer> Deserializers
    {
        get
        {
            if (deserializers == null)
            {
                deserializers = new Dictionary<Type, Deserializer>();
                deserializers.Add(typeof(char), CharDeserializer);
                deserializers.Add(typeof(string), StringDeserializer);
                deserializers.Add(typeof(bool), BooleanDeserializer);
                deserializers.Add(typeof(int), IntDeserializer);
                deserializers.Add(typeof(short), ShortDeserializer);
                deserializers.Add(typeof(ConnectionId), ConnectionIdDeserializer);
                deserializers.Add(typeof(float), FloatDeserializer);
                deserializers.Add(typeof(Vector3), Vector3Deserializer);
                deserializers.Add(typeof(Quaternion), QuaternionDeserializer);
                deserializers.Add(typeof(InstantiationMessage), InstantiationMessageDeserializer);
                deserializers.Add(typeof(Team), TeamDeserializer);
                deserializers.Add(typeof(MatchManager.State), MMStateDeserializer);
                deserializers.Add(typeof(Vector2), Vector2Deserializer);
                deserializers.Add(typeof(Color), ColorDeserializer);

            }
            return deserializers;
        }
    }

    public static Dictionary<Type, Serializer> serializers;
    public static Dictionary<Type, Serializer> Serializers
    {
        get
        {
            if (serializers == null)
            {
                serializers = new Dictionary<Type, Serializer>();
                serializers.Add(typeof(char), CharSerializer);
                serializers.Add(typeof(string), StringSerializer);
                serializers.Add(typeof(bool), BooleanSerializer);
                serializers.Add(typeof(int), IntSerializer);
                serializers.Add(typeof(short), ShortSerializer);
                serializers.Add(typeof(ConnectionId), ConnectionIdSerializer);
                serializers.Add(typeof(float), FloatSerializer);
                serializers.Add(typeof(Vector3), Vector3Serializer);
                serializers.Add(typeof(Quaternion), QuaternionSerializer);
                serializers.Add(typeof(InstantiationMessage), InstantiationMessageSerializer);
                serializers.Add(typeof(Team), TeamSerializer);
                serializers.Add(typeof(MatchManager.State), MMStateSerializer);
                serializers.Add(typeof(Vector2), Vector2Serializer);
                serializers.Add(typeof(Color), ColorSerializer);
            }
            return serializers;
        }
    }

    #region Deserializers
    public static object CharDeserializer(byte[] data, ref int currentIndex)
    {
        char result = BitConverter.ToChar(data, currentIndex);
        currentIndex += 2;
        return result;
    }

    public static object StringDeserializer(byte[] data, ref int currentIndex)
    {
        int length = BitConverter.ToInt16(data, currentIndex);
        currentIndex += 2;
        string result = System.Text.Encoding.UTF8.GetString(data, currentIndex, length);
        currentIndex += length;
        return result;
    }

    public static object BooleanDeserializer(byte[] data, ref int currentIndex)
    {
        bool result = BitConverter.ToBoolean(data, currentIndex);
        currentIndex++;
        return result;
    }

    public static object FloatDeserializer(byte[] data, ref int currentIndex)
    {
        float result = BitConverter.ToSingle(data, currentIndex);
        currentIndex += 4;
        return result;
    }

    public static object IntDeserializer(byte[] data, ref int currentIndex)
    {
        int result = BitConverter.ToInt32(data, currentIndex);
        currentIndex += 4;
        return result;
    }

    public static object ShortDeserializer(byte[] data, ref int currentIndex)
    {
        short result = BitConverter.ToInt16(data, currentIndex);
        currentIndex += 2;
        return result;
    }

    public static object ConnectionIdDeserializer(byte[] data, ref int currentIndex)
    {
        short result = BitConverter.ToInt16(data, currentIndex);
        currentIndex += 2;
        return new ConnectionId(result);
    }

    public static Vector3 DeserializeVector3(byte[] data, ref int currentIndex)
    {
        float x = BitConverter.ToSingle(data, currentIndex);
        currentIndex += 4;
        float y = BitConverter.ToSingle(data, currentIndex);
        currentIndex += 4;
        float z = BitConverter.ToSingle(data, currentIndex);
        currentIndex += 4;
        return new Vector3(x, y, z);
    }

    public static object Vector3Deserializer(byte[] data, ref int currentIndex)
    {
        return DeserializeVector3(data, ref currentIndex);
    }

    public static Quaternion DeserializeQuaternion(byte[] data, ref int currentIndex)
    {
        float x = BitConverter.ToSingle(data, currentIndex);
        currentIndex += 4;
        float y = BitConverter.ToSingle(data, currentIndex);
        currentIndex += 4;
        float z = BitConverter.ToSingle(data, currentIndex);
        currentIndex += 4;
        float w = BitConverter.ToSingle(data, currentIndex);
        currentIndex += 4;
        return new Quaternion(x, y, z, w);
    }

    public static object QuaternionDeserializer(byte[] data, ref int currentIndex)
    {
        return DeserializeQuaternion(data, ref currentIndex);
    }

    public static object InstantiationMessageDeserializer(byte[] data, ref int currentIndex)
    {
        string path = (string)StringDeserializer(data, ref currentIndex);
        short newViewId = (short)ShortDeserializer(data, ref currentIndex);
        Vector3 position = (Vector3)Vector3Deserializer(data, ref currentIndex);
        Quaternion rotation = (Quaternion)QuaternionDeserializer(data, ref currentIndex);
        object[] parameters = ObjectArrayDeserializer(data, currentIndex);
        return new InstantiationMessage(newViewId, path, position, rotation, parameters);
    }

    public static object TeamDeserializer(byte[] data, ref int currentIndex)
    {
        Team team = (Team)data[currentIndex];
        currentIndex++;
        return team;
    }

    public static object MMStateDeserializer(byte[] data, ref int currentIndex)
    {
        MatchManager.State state = (MatchManager.State)data[currentIndex];
        currentIndex++;
        return state;
    }

    public static object ArrayDeserializer(Type type, byte[] data, ref int currentIndex)
    {
        Deserializer deserializer;
        if (Deserializers.TryGetValue(type.GetElementType(), out deserializer))
        {
            short length = (short)ShortDeserializer(data, ref currentIndex);
            object[] result = new object[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = deserializer(data, ref currentIndex);
                result[i] = Convert.ChangeType(result[i], type.GetElementType());
            }

            var realTypeResult = Array.CreateInstance(type.GetElementType(), result.Length);
            Array.Copy(result, realTypeResult, result.Length);
            return realTypeResult;
        }
        else
        {
            Debug.LogError("There are no serializers for this type " + type.GetElementType());
            return null;
        }
    }

    public static Vector2 DeserializeVector2(byte[] data, ref int currentIndex)
    {
        float x = BitConverter.ToSingle(data, currentIndex);
        currentIndex += 4;
        float y = BitConverter.ToSingle(data, currentIndex);
        currentIndex += 4;
        return new Vector2(x, y);
    }

    public static object Vector2Deserializer(byte[] data, ref int currentIndex)
    {
        return DeserializeVector2(data, ref currentIndex);
    }

    public static object ColorDeserializer(byte[] data, ref int currentIndex)
    {
        float r = BitConverter.ToSingle(data, currentIndex);
        currentIndex += 4;
        float g = BitConverter.ToSingle(data, currentIndex);
        currentIndex += 4;
        float b = BitConverter.ToSingle(data, currentIndex);
        currentIndex += 4;
        float a = BitConverter.ToSingle(data, currentIndex);
        currentIndex += 4;
        return new Color(r,g,b,a);
    }


    //This method assume that the array is the last item in the byte array
    private static object[] ObjectArrayDeserializer(byte[] data, int startIndex)
    {
        List<object> objects = new List<object>();
        int currentIndex = startIndex;
        while (currentIndex < data.Length)
        {
            short typeId = (short)ShortDeserializer(data, ref currentIndex);
            Type t;
            if (TypeIds.Forward.TryGetValue(typeId, out t))
            {
                Deserializer deserializer;
                if (Deserializers.TryGetValue(t, out deserializer))
                {
                    objects.Add(deserializer(data, ref currentIndex));
                }
                else
                {
                    Debug.LogError("This type has no deserializer " + t);
                }
            }
            else
            {
                Debug.LogError("This type isn't handled by the network " + typeId);
            }
        }
        return objects.ToArray();
    }
    #endregion

    #region Serializers
    public static byte[] CharSerializer(object parameter)
    {
        return BitConverter.GetBytes((char)parameter);
    }

    public static byte[] StringSerializer(object parameter)
    {
        byte[] s = System.Text.Encoding.UTF8.GetBytes((string)parameter);
        Assert.IsTrue(s.Length < short.MaxValue);
        byte[] length = BitConverter.GetBytes((short)s.Length);
        return length.Concatenate(s);
    }

    public static byte[] BooleanSerializer(object parameter)
    {
        return BitConverter.GetBytes((bool)parameter);
    }

    public static byte[] FloatSerializer(object parameter)
    {
        return BitConverter.GetBytes((float)parameter);

    }

    public static byte[] IntSerializer(object parameter)
    {
        return BitConverter.GetBytes((int)parameter);

    }

    public static byte[] ShortSerializer(object parameter)
    {
        return BitConverter.GetBytes((short)parameter);
    }

    public static byte[] ConnectionIdSerializer(object parameter)
    {
        return BitConverter.GetBytes(((ConnectionId)parameter).id);
    }

    public static byte[] SerializeVector3(Vector3 v)
    {
        byte[] data = BitConverter.GetBytes(v.x);
        data = data.Concatenate(BitConverter.GetBytes(v.y));
        return data.Concatenate(BitConverter.GetBytes(v.z));
    }

    public static byte[] Vector3Serializer(object parameter)
    {
        Vector3 v = (Vector3)parameter;
        return SerializeVector3(v);
    }

    public static byte[] SerializeQuaternion(Quaternion q)
    {
        byte[] data = BitConverter.GetBytes(q.x);
        data = data.Concatenate(BitConverter.GetBytes(q.y));
        data = data.Concatenate(BitConverter.GetBytes(q.z));
        return data.Concatenate(BitConverter.GetBytes(q.w));
    }

    public static byte[] QuaternionSerializer(object parameter)
    {
        Quaternion v = (Quaternion)parameter;
        return SerializeQuaternion(v);
    }

    public static byte[] InstantiationMessageSerializer(object parameter)
    {
        InstantiationMessage iMessage = (InstantiationMessage)parameter;
        byte[] data = StringSerializer(iMessage.path);
        data = data.Concatenate(ShortSerializer(iMessage.newViewId));
        data = data.Concatenate(SerializeVector3(iMessage.position));
        data = data.Concatenate(QuaternionSerializer(iMessage.rotation));
        data = data.Concatenate(ObjectArraySerializer(iMessage.initialisationParameters));
        return data;
    }

    private static byte[] ObjectArraySerializer(object[] parameters)
    {
        byte[] data = new byte[0];
        foreach (var parameter in parameters)
        {
            Type t = parameter.GetType();
            short typeId;
            if (TypeIds.Reverse.TryGetValue(t, out typeId))
            {
                Serializer serializer;
                if (Serializers.TryGetValue(t, out serializer))
                {
                    data = data.Concatenate(BitConverter.GetBytes(typeId));
                    data = data.Concatenate(serializer(parameter));
                }
                else
                {
                    Debug.LogError("This type has no serializer " + t);
                }
            }
            else
            {
                Debug.LogError("This type isn't handled by the network " + t);
            }
        }
        return data;
    }

    public static byte[] TeamSerializer(object parameter)
    {
        Team team = (Team)parameter;
        return new byte[1] { (byte)team };
    }

    public static byte[] MMStateSerializer(object parameter)
    {
        MatchManager.State state = (MatchManager.State)parameter;
        return new byte[1] { (byte)state };
    }

    public static byte[] SerializeVector2(Vector2 v)
    {
        byte[] data = BitConverter.GetBytes(v.x);
        return data.Concatenate(BitConverter.GetBytes(v.y));
    }

    public static byte[] Vector2Serializer(object parameter)
    {
        Vector2 v = (Vector2)parameter;
        return SerializeVector2(v);
    }

    public static byte[] ColorSerializer(object parameter)
    {
        Color c = (Color)parameter;
        byte[] data = BitConverter.GetBytes(c.r);
        data = data.Concatenate(BitConverter.GetBytes(c.g));
        data = data.Concatenate(BitConverter.GetBytes(c.b));
        return data.Concatenate(BitConverter.GetBytes(c.a));
    }

    public static byte[] ArraySerializer(object parameter)
    {
        byte[] data;
        Array array = (Array)parameter;
        Type elementType = array.GetType().GetElementType();
        Serializer serializer;
        if (Serializers.TryGetValue(elementType, out serializer))
        {
            Assert.IsTrue(array.Length < short.MaxValue);
            data = ShortSerializer((short)array.Length);
            foreach (var element in array)
            {
                data = data.Concatenate(serializer(element));
            }
        }
        else
        {
            Debug.LogError("There are no serializers for this type " + elementType);
            return null;
        }
        return data;
    }
    #endregion
}
