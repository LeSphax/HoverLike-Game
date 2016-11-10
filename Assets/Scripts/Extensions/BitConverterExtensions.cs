using System;
using UnityEngine;
using UnityEngine.Assertions;

public static class BitConverterExtensions
{

    public static Vector3 ToVector3(byte[] data, int currentIndex)
    {
        float x = BitConverter.ToSingle(data, currentIndex);
        float y = BitConverter.ToSingle(data, currentIndex + 4);
        float z = BitConverter.ToSingle(data, currentIndex + 8);
        return new Vector3(x, y, z);
    }

    public static byte[] GetBytes(Vector3 v)
    {
        byte[] data;
        Assert.IsTrue(BitConverter.GetBytes(v.x).Length == 4, "The float length is not what we expected " + BitConverter.GetBytes(v.x).Length);
        data = ArrayExtensions.Concatenate(BitConverter.GetBytes(v.x), BitConverter.GetBytes(v.y));
        return ArrayExtensions.Concatenate(data, BitConverter.GetBytes(v.z));
    }

}
