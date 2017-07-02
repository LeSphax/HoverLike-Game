using System.Text;

public static class ByteArrayExtensions
{

    public static string ToString(this byte[] array)
    {
        return Encoding.UTF8.GetString(array);
    }
}
