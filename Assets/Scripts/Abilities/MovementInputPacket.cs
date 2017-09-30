using System;
using UnityEngine;

public class MovementInputPacket
{
    public bool up;
    public bool down;
    public bool left;
    public bool right;

    public MovementInputPacket(bool up, bool down, bool left, bool right)
    {
        this.up = up;
        this.down = down;
        this.left = left;
        this.right = right;
    }

    public byte[] Serialize()
    {
        byte[] data = BitConverter.GetBytes(up);
        data = ArrayExtensions.Concatenate(data, BitConverter.GetBytes(down));
        data = ArrayExtensions.Concatenate(data, BitConverter.GetBytes(left));
        return ArrayExtensions.Concatenate(data, BitConverter.GetBytes(right));
    }

    public static MovementInputPacket Deserialize(byte[] data)
    {
        return new MovementInputPacket(
            BitConverter.ToBoolean(data, 0),
            BitConverter.ToBoolean(data, 1),
            BitConverter.ToBoolean(data, 2),
            BitConverter.ToBoolean(data, 3)
            );
    }

    public Vector3 GetDirection()
    {
        Vector3 direction = Vector3.zero;
        direction = up ? direction + Vector3.right : direction;
        direction = down ? direction + Vector3.left : direction;
        direction = left ? direction + Vector3.forward : direction;
        direction = right ? direction + Vector3.back : direction;
        return direction;
    }
}
