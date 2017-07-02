using System;

namespace Byn.Net
{
	[Serializable]
	public struct ConnectionId
	{
		public static readonly ConnectionId INVALID = new ConnectionId
		{
			id = -1
		};

		public short id;

		public ConnectionId(short lId)
		{
			this.id = lId;
		}

		public override bool Equals(object obj)
		{
			return obj is ConnectionId && (ConnectionId)obj == this;
		}

		public bool IsValid()
		{
			return this != ConnectionId.INVALID;
		}

		public override int GetHashCode()
		{
			return this.id.GetHashCode();
		}

		public static bool operator ==(ConnectionId i1, ConnectionId i2)
		{
			return i1.id == i2.id;
		}

		public static bool operator !=(ConnectionId i1, ConnectionId i2)
		{
			return !(i1 == i2);
		}

		public static bool operator <(ConnectionId i1, ConnectionId i2)
		{
			return i1.id < i2.id;
		}

		public static bool operator >(ConnectionId i1, ConnectionId i2)
		{
			return i1.id > i2.id;
		}

		public static bool operator <=(ConnectionId i1, ConnectionId i2)
		{
			return i1.id <= i2.id;
		}

		public static bool operator >=(ConnectionId i1, ConnectionId i2)
		{
			return i1.id >= i2.id;
		}

		public override string ToString()
		{
			return this.id.ToString();
		}
	}
}
