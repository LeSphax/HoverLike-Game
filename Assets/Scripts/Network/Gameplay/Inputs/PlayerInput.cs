using System;

public abstract class PlayerInput<T> where T : PlayerInput<T>
{
    public float timeSent;

    public byte[] Serialize()
    {
        byte[] data = BitConverter.GetBytes(true);
        return ArrayExtensions.Concatenate(data, SerializeParameters());
    }

    public void Deserialize(byte[] data, int currentIndex)
    {
        bool inputPresent = BitConverter.ToBoolean(data, currentIndex);
        if (inputPresent)
        {
            DeserializeParameters(data, currentIndex + 1);
        }
    }

    protected abstract byte[] SerializeParameters();
    protected abstract void DeserializeParameters(byte[] data, int currentIndex);
}
