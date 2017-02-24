using Byn.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using WebRtcCSharp;

namespace Byn.Net.Native
{
	public class WebRtcDataPeer : AWebRtcPeer
	{
		private ConnectionId mConnectionId;

		private SignalingInfo mInfo;

		private Queue<NetworkEvent> mEvents = new Queue<NetworkEvent>();

		private static readonly string sLabelReliable = "reliable";

		private static readonly string sLabelUnreliable = "unreliable";

		private bool mRtcReliableDataChannelReady;

		private bool mRtcUnreliableDataChannelReady;

		private RTCDataChannel mReliableDataChannel;

		private RTCDataChannel mUnreliableDataChannel;

		public ConnectionId ConnectionId
		{
			get
			{
				return this.mConnectionId;
			}
		}

		public SignalingInfo SignalingInfo
		{
			get
			{
				return this.mInfo;
			}
			set
			{
				this.mInfo = value;
			}
		}

		public WebRtcDataPeer(ConnectionId ownConnectionId, string[] urls, NativeWebRtcNetworkFactory factory) : base(urls, factory)
		{
			this.mConnectionId = ownConnectionId;
		}

		protected override void OnSetup()
		{
			this.mPeer.OnDataChannel = new Action<RTCDataChannel>(this.OnDataChannel);
		}

		protected override void OnStartSignaling()
		{
			DataChannelInit reliable = new DataChannelInit();
			reliable.ordered = true;
			this.mReliableDataChannel = this.mPeer.CreateDataChannel(WebRtcDataPeer.sLabelReliable, reliable);
			this.RegisterObserverReliable();
			DataChannelInit unreliable = new DataChannelInit();
			unreliable.ordered = false;
			unreliable.maxRetransmits = 0;
			this.mUnreliableDataChannel = this.mPeer.CreateDataChannel(WebRtcDataPeer.sLabelUnreliable, unreliable);
			this.RegisterObserverUnreliable();
		}

		protected override void OnCleanup()
		{
			if (this.mReliableDataChannel != null)
			{
				this.mReliableDataChannel.Dispose();
			}
			this.mReliableDataChannel = null;
			if (this.mUnreliableDataChannel != null)
			{
				this.mUnreliableDataChannel.Dispose();
			}
			this.mUnreliableDataChannel = null;
		}

		private void RegisterObserverReliable()
		{
			this.mReliableDataChannel.OnMessage = new Action<DataBuffer>(this.ReliableDataChannel_OnMessage);
			this.mReliableDataChannel.OnOpen = new Action(this.ReliableDataChannel_OnOpen);
			this.mReliableDataChannel.OnClose = new Action(this.ReliableDataChannel_OnClose);
			this.mReliableDataChannel.OnError = new Action<string>(this.ReliableDataChannel_OnError);
		}

		private void RegisterObserverUnreliable()
		{
			this.mUnreliableDataChannel.OnMessage = new Action<DataBuffer>(this.UnreliableDataChannel_OnMessage);
			this.mUnreliableDataChannel.OnOpen = new Action(this.UnreliableDataChannel_OnOpen);
			this.mUnreliableDataChannel.OnClose = new Action(this.UnreliableDataChannel_OnClose);
			this.mUnreliableDataChannel.OnError = new Action<string>(this.UnreliableDataChannel_OnError);
		}

		public void SendData(byte[] data, int offset, int length, bool reliable)
		{
			if (reliable)
			{
				this.mReliableDataChannel.Send(data, (uint)offset, (uint)length);
				return;
			}
			this.mUnreliableDataChannel.Send(data, (uint)offset, (uint)length);
		}

		public bool DequeueEvent(out NetworkEvent ev)
		{
			Queue<NetworkEvent> obj = this.mEvents;
			lock (obj)
			{
				if (this.mEvents.Count > 0)
				{
					ev = this.mEvents.Dequeue();
					return true;
				}
			}
			ev = default(NetworkEvent);
			return false;
		}

		private void Enqueue(NetworkEvent evt)
		{
			Queue<NetworkEvent> obj = this.mEvents;
			lock (obj)
			{
				this.mEvents.Enqueue(evt);
			}
		}

		private void OnDataChannel(RTCDataChannel newChannel)
		{
			if (newChannel.label() == WebRtcDataPeer.sLabelReliable)
			{
				this.mReliableDataChannel = newChannel;
				this.RegisterObserverReliable();
				return;
			}
			if (newChannel.label() == WebRtcDataPeer.sLabelUnreliable)
			{
				this.mUnreliableDataChannel = newChannel;
				this.RegisterObserverUnreliable();
				return;
			}
			SLog.LE("Datachannel with unexpected label " + newChannel.label(), new string[0]);
		}

		private MessageDataBuffer ToMessageDataBuffer(DataBuffer buff)
		{
			ByteArrayBuffer csbuffer = ByteArrayBuffer.Get((int)buff.size(), false);
			WebRtcWrap.Copy(buff, csbuffer.Buffer, buff.size());
			csbuffer.ContentLength = (int)buff.size();
			return csbuffer;
		}

		private void RtcOnMessageReceived(DataBuffer buff, bool reliable)
		{
			NetEventType eventType = NetEventType.UnreliableMessageReceived;
			if (reliable)
			{
				eventType = NetEventType.ReliableMessageReceived;
			}
			this.Enqueue(new NetworkEvent(eventType, this.mConnectionId, this.ToMessageDataBuffer(buff)));
		}

		private void ReliableDataChannel_OnMessage(DataBuffer buff)
		{
			this.RtcOnMessageReceived(buff, true);
		}

		private void ReliableDataChannel_OnOpen()
		{
			SLog.L("mReliableDataChannelReady", new string[0]);
			this.mRtcReliableDataChannelReady = true;
			if (this.IsRtcReady())
			{
				base.RtcSetConnected();
				SLog.L("Fully connected", new string[0]);
			}
		}

		private void ReliableDataChannel_OnClose()
		{
			SLog.L("ReliableDataChannel_OnClose", new string[0]);
			base.RtcSetClosed();
		}

		private void ReliableDataChannel_OnError(string errorMsg)
		{
			SLog.LE(errorMsg, new string[0]);
			base.RtcSetClosed();
		}

		private void UnreliableDataChannel_OnMessage(DataBuffer buff)
		{
			this.RtcOnMessageReceived(buff, false);
		}

		private void UnreliableDataChannel_OnOpen()
		{
			SLog.L("mUnreliableDataChannelReady", new string[0]);
			this.mRtcUnreliableDataChannelReady = true;
			if (this.IsRtcReady())
			{
				base.RtcSetConnected();
				SLog.L("Fully connected", new string[0]);
			}
		}

		private void UnreliableDataChannel_OnClose()
		{
			SLog.L("UnreliableDataChannel_OnClose", new string[0]);
			base.RtcSetClosed();
		}

		private void UnreliableDataChannel_OnError(string errorMsg)
		{
			SLog.LE(errorMsg, new string[0]);
			base.RtcSetClosed();
		}

		protected virtual bool IsRtcReady()
		{
			return this.mRtcReliableDataChannelReady && this.mRtcUnreliableDataChannelReady;
		}
	}
}
