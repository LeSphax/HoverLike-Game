using System;

namespace Byn.Net.Native
{
	public class SignalingInfo
	{
		private bool mSignalingConnected;

		private ConnectionId mConnectionId;

		private bool mIsIncoming;

		private DateTime mCreationTime;

		public bool IsSignalingConnected
		{
			get
			{
				return this.mSignalingConnected;
			}
		}

		public ConnectionId ConnectionId
		{
			get
			{
				return this.mConnectionId;
			}
		}

		public bool IsIncoming
		{
			get
			{
				return this.mIsIncoming;
			}
		}

		public int CreationTimeMs
		{
			get
			{
				return (int)(DateTime.Now - this.mCreationTime).TotalMilliseconds;
			}
		}

		public SignalingInfo(ConnectionId id, bool isIncoming, DateTime timeStamp)
		{
			this.mConnectionId = id;
			this.mIsIncoming = isIncoming;
			this.mCreationTime = timeStamp;
			this.mSignalingConnected = true;
		}

		public void SignalingDisconnected()
		{
			this.mSignalingConnected = false;
		}
	}
}
