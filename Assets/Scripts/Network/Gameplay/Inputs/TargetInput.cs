using System;
using UnityEngine;

public class TargetInput : PlayerInput<TargetInput>
{
    public Vector3 targetPosition;

    protected override void DeserializeParameters(byte[] data, int currentIndex)
    {
        targetPosition = BitConverterExtensions.ToVector3(data, currentIndex);
    }

    protected override byte[] SerializeParameters()
    {
        return BitConverterExtensions.GetBytes(targetPosition);
    }
}
