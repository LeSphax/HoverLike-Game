using System;
using System.Text;

namespace Byn.Net
{
	public struct NetworkEvent
	{
		private NetEventType type;

		private ConnectionId connectionId;

		private object data;

		public NetEventType Type
		{
			get
			{
				return this.type;
			}
		}

		public ConnectionId ConnectionId
		{
			get
			{
				return this.connectionId;
			}
		}

		public object RawData
		{
			get
			{
				return this.data;
			}
		}

		public MessageDataBuffer MessageData
		{
			get
			{
				return this.data as MessageDataBuffer;
			}
		}

		public string Info
		{
			get
			{
				return this.data as string;
			}
		}

		public byte[] GetDataAsByteArray()
		{
			if (this.MessageData != null)
			{
				byte[] arr = new byte[this.MessageData.ContentLength];
				Array.Copy(this.MessageData.Buffer, this.MessageData.Offset, arr, 0, this.MessageData.ContentLength);
				return arr;
			}
			return null;
		}

		public NetworkEvent(NetEventType t)
		{
			this.type = t;
			this.connectionId = ConnectionId.INVALID;
			this.data = null;
		}

		public NetworkEvent(NetEventType t, ConnectionId conId, object dt)
		{
			this.type = t;
			this.connectionId = conId;
			this.data = dt;
			if (this.data != null && !(this.data is ByteArrayBuffer) && !(this.data is string))
			{
				throw new ArgumentException("data can only be ByteArrayBuffer or string");
			}
		}

		public override string ToString()
		{
			StringBuilder datastring = new StringBuilder();
			datastring.Append("NetworkEvent type: ");
			datastring.Append(this.type);
			datastring.Append(" connection: ");
			datastring.Append(this.connectionId);
			datastring.Append(" data: ");
			if (this.data is ByteArrayBuffer)
			{
				ByteArrayBuffer msg = (ByteArrayBuffer)this.data;
				datastring.Append(BitConverter.ToString(msg.array, msg.Offset, msg.PositionWriteAbsolute));
			}
			else
			{
				datastring.Append(this.data);
			}
			return datastring.ToString();
		}

		public static NetworkEvent FromByteArray(byte[] arr)
		{
			NetEventType type = (NetEventType)arr[0];
			NetEventDataType dataType = (NetEventDataType)arr[1];
			ConnectionId conId = new ConnectionId(BitConverter.ToInt16(arr, 2));
			object data = null;
			if (dataType == NetEventDataType.ByteArray)
			{
				uint length = BitConverter.ToUInt32(arr, 4);
				ByteArrayBuffer buff = ByteArrayBuffer.Get((int)length, false);
				for (uint i = 0u; i < length; i += 1u)
				{
					buff.Buffer[(int)(checked((IntPtr)(unchecked((long)buff.Offset + (long)((ulong)i)))))] = arr[(int)(8u + i)];
				}
				buff.PositionWriteRelative = (int)length;
				data = buff;
			}
			else if (dataType == NetEventDataType.UTF16String)
			{
				uint length2 = BitConverter.ToUInt32(arr, 4);
				length2 *= 2u;
				data = Encoding.Unicode.GetString(arr, 8, (int)length2);
			}
			return new NetworkEvent(type, conId, data);
		}

		public static byte[] ToByteArray(NetworkEvent evt)
		{
			bool falseUtf16 = true;
			uint length = 4u;
			byte dataType = 0;
			if (evt.data is ByteArrayBuffer)
			{
				dataType = 1;
				length += (uint)(4 + evt.MessageData.ContentLength);
			}
			else if (evt.data is string)
			{
				dataType = 2;
				string str = evt.data as string;
				if (falseUtf16)
				{
					length += (uint)(4 + str.Length * 2);
				}
				else
				{
					length += (uint)(4 + Encoding.Unicode.GetByteCount(str));
				}
			}
			byte[] arr = new byte[length];
			arr[0] = (byte)evt.type;
			arr[1] = dataType;
			byte[] id = BitConverter.GetBytes(evt.connectionId.id);
			arr[2] = id[0];
			arr[3] = id[1];
			if (evt.data is ByteArrayBuffer)
			{
				byte[] dataLengthBytes = BitConverter.GetBytes(evt.MessageData.ContentLength);
				arr[4] = dataLengthBytes[0];
				arr[5] = dataLengthBytes[1];
				arr[6] = dataLengthBytes[2];
				arr[7] = dataLengthBytes[3];
				int dataOffset = evt.MessageData.Offset;
				for (int i = 0; i < evt.MessageData.ContentLength; i++)
				{
					arr[8 + i] = evt.MessageData.Buffer[dataOffset + i];
				}
			}
			else if (evt.data is string)
			{
				if (falseUtf16)
				{
					string str2 = evt.data as string;
					byte[] dataLengthBytes2 = BitConverter.GetBytes(str2.Length);
					arr[4] = dataLengthBytes2[0];
					arr[5] = dataLengthBytes2[1];
					arr[6] = dataLengthBytes2[2];
					arr[7] = dataLengthBytes2[3];
					for (int j = 0; j < str2.Length; j++)
					{
						ushort val = (ushort)str2[j];
						arr[8 + j * 2] = (byte)val;
						arr[8 + j * 2 + 1] = (byte)(val >> 8);
					}
				}
				else
				{
					string str3 = evt.data as string;
					byte[] dataLengthBytes3 = BitConverter.GetBytes(Encoding.Unicode.GetByteCount(str3) / 2);
					arr[4] = dataLengthBytes3[0];
					arr[5] = dataLengthBytes3[1];
					arr[6] = dataLengthBytes3[2];
					arr[7] = dataLengthBytes3[3];
					Encoding.Unicode.GetBytes(str3, 0, str3.Length, arr, 8);
				}
			}
			return arr;
		}
	}
}
