using System;
using System.Text;

namespace Byn.Net
{
	public static class MessageDataBufferExt
	{
		public static string AsStringUTF8(this MessageDataBuffer buffer)
		{
			if (buffer.ContentLength == 0)
			{
				return "";
			}
			return Encoding.UTF8.GetString(buffer.Buffer, buffer.Offset, buffer.ContentLength);
		}

		public static string AsStringUnicode(this MessageDataBuffer buffer)
		{
			if (buffer.ContentLength == 0)
			{
				return "";
			}
			return Encoding.Unicode.GetString(buffer.Buffer, buffer.Offset, buffer.ContentLength);
		}

		public static byte[] Copy(this MessageDataBuffer buffer)
		{
			byte[] copy = new byte[buffer.ContentLength];
			Array.Copy(buffer.Buffer, buffer.Offset, copy, 0, buffer.ContentLength);
			return copy;
		}

        public static MessageDataBuffer StringToBuffer(string msg)
        {
            int byteLen = Encoding.Unicode.GetByteCount(msg);
            ByteArrayBuffer buff = ByteArrayBuffer.Get(byteLen, false);
            Encoding.Unicode.GetBytes(msg, 0, msg.Length, buff.array, buff.Offset);
            buff.PositionWriteRelative = byteLen;
            return buff;
        }
    }
}
