//using Byn.Common;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Byn.Net
//{
//	public class LocalNetwork : ServerConnection, INetwork, IDisposable
//	{
//		private class WeakRef<T> where T : class
//		{
//			private WeakReference mRef;

//			public WeakRef(T val)
//			{
//				this.mRef = new WeakReference(val);
//			}

//			public T Get()
//			{
//				if (this.mRef.IsAlive)
//				{
//					return this.mRef.Target as T;
//				}
//				return default(T);
//			}
//		}

//		private static int sNextId = 1;

//		private static Dictionary<string, LocalNetwork.WeakRef<LocalNetwork>> mServers = new Dictionary<string, LocalNetwork.WeakRef<LocalNetwork>>();

//		private int mId;

//		private ConnectionId mNextNetworkId = new ConnectionId
//		{
//			id = 1
//		};

//		private string mServerAddress;

//		private Queue<NetworkEvent> mEvents = new Queue<NetworkEvent>();

//		private Dictionary<ConnectionId, LocalNetwork.WeakRef<LocalNetwork>> mConnectionNetwork = new Dictionary<ConnectionId, LocalNetwork.WeakRef<LocalNetwork>>();

//		private bool disposedValue;

//		public IList<ConnectionId> Connections
//		{
//			get
//			{
//				return this.mConnectionNetwork.Keys.ToList<ConnectionId>();
//			}
//		}

//		public bool IsServer
//		{
//			get
//			{
//				return this.mServerAddress != null;
//			}
//		}

//		public LocalNetwork()
//		{
//			this.mId = LocalNetwork.sNextId;
//			LocalNetwork.sNextId++;
//		}

//        public void LeaveRoom()
//        {
//            if (this.IsServer)
//            {
//                this.Enqueue(NetEventType.ServerClosed, ConnectionId.INVALID, this.mServerAddress);
//                LocalNetwork.mServers.Remove(this.mServerAddress);
//                this.mServerAddress = null;
//            }
//        }

//        public void CreateRoom(string serverAddress = null)
//        {
//            if (serverAddress == null)
//            {
//                serverAddress = string.Concat(this.mId);
//            }
//            if (LocalNetwork.mServers.ContainsKey(serverAddress) && LocalNetwork.mServers[serverAddress].Get() != null)
//            {
//                this.Enqueue(NetEventType.ServerConnectionFailed, ConnectionId.INVALID, serverAddress);
//                return;
//            }
//            LocalNetwork.mServers[serverAddress] = new LocalNetwork.WeakRef<LocalNetwork>(this);
//            this.mServerAddress = serverAddress;
//            this.Enqueue(NetEventType.ServerInitialized, ConnectionId.INVALID, serverAddress);
//        }

//		public void ConnectToServer()
//		{
//		}

//		public void Disconnect()
//		{
			
//		}

//		public ConnectionId ConnectToRoom(string address)
//		{
//			ConnectionId connectionId = this.NextConnectionId();
//			bool sucessful = false;
//			if (LocalNetwork.mServers.ContainsKey(address))
//			{
//				LocalNetwork server = LocalNetwork.mServers[address].Get();
//				if (server != null)
//				{
//					server.ConnectClient(this);
//					this.mConnectionNetwork[connectionId] = LocalNetwork.mServers[address];
//					this.Enqueue(NetEventType.NewConnection, connectionId, null);
//					sucessful = true;
//				}
//			}
//			if (!sucessful)
//			{
//				this.Enqueue(NetEventType.ConnectionFailed, connectionId, "Couldn't connect to the given server with id " + address);
//			}
//			return connectionId;
//		}

//		public void Shutdown()
//		{
//			foreach (ConnectionId v in this.mConnectionNetwork.Keys.ToList<ConnectionId>())
//			{
//				this.Disconnect(v);
//			}
//			this.mConnectionNetwork.Clear();
//			this.Disconnect();
//		}

//		public void SendData(ConnectionId userId, byte[] data, int offset, int length, bool reliable)
//		{
//			LocalNetwork.WeakRef<LocalNetwork> netref;
//			if (this.mConnectionNetwork.TryGetValue(userId, out netref))
//			{
//				this.mConnectionNetwork[userId].Get().ReceiveData(this, data, offset, length, reliable);
//			}
//		}

//		public void Update()
//		{
//			this.CleanupWreakReferences();
//		}

//		public bool Dequeue(out NetworkEvent evt)
//		{
//			if (this.mEvents.Count == 0)
//			{
//				evt = default(NetworkEvent);
//				return false;
//			}
//			evt = this.mEvents.Dequeue();
//			return true;
//		}

//		public bool Peek(out NetworkEvent evt)
//		{
//			if (this.mEvents.Count == 0)
//			{
//				evt = default(NetworkEvent);
//				return false;
//			}
//			evt = this.mEvents.Peek();
//			return true;
//		}

//		public void Flush()
//		{
//		}

//		public void Disconnect(ConnectionId id)
//		{
//			if (this.mConnectionNetwork.ContainsKey(id))
//			{
//				LocalNetwork other = this.mConnectionNetwork[id].Get();
//				if (other != null)
//				{
//					other.InternalDisconnect(this);
//					this.InternalDisconnect(id);
//					return;
//				}
//				this.CleanupWreakReferences();
//			}
//		}

//		private ConnectionId FindConnectionId(LocalNetwork network)
//		{
//			foreach (KeyValuePair<ConnectionId, LocalNetwork.WeakRef<LocalNetwork>> kvp in this.mConnectionNetwork)
//			{
//				if (kvp.Value.Get() == network)
//				{
//					return kvp.Key;
//				}
//			}
//			return ConnectionId.INVALID;
//		}

//		private ConnectionId NextConnectionId()
//		{
//			ConnectionId arg_17_0 = this.mNextNetworkId;
//			this.mNextNetworkId.id = (short)(this.mNextNetworkId.id + 1);
//			return arg_17_0;
//		}

//		private void ConnectClient(LocalNetwork client)
//		{
//			if (!this.IsServer)
//			{
//				throw new InvalidOperationException();
//			}
//			ConnectionId nextId = this.NextConnectionId();
//			this.mConnectionNetwork[nextId] = new LocalNetwork.WeakRef<LocalNetwork>(client);
//			this.Enqueue(NetEventType.NewConnection, nextId, null);
//		}

//		private void Enqueue(NetEventType type, ConnectionId id, object data)
//		{
//			NetworkEvent ev = new NetworkEvent(type, id, data);
//			this.mEvents.Enqueue(ev);
//		}

//		private void ReceiveData(LocalNetwork network, byte[] data, int offset, int length, bool reliable)
//		{
//			ConnectionId userId = this.FindConnectionId(network);
//			ByteArrayBuffer buffer = ByteArrayBuffer.Get(length, false);
//			buffer.CopyFrom(data, offset, length);
//			if (buffer.PositionReadRelative != 0)
//			{
//				SLog.LE("Received an invalid message. PositionReadRelative must be 0 but is " + buffer.PositionReadRelative, new string[0]);
//			}
//			if (buffer.PositionWriteRelative == 0)
//			{
//				SLog.LE("Received an invalid message. Message empty.", new string[0]);
//			}
//			NetEventType type = NetEventType.UnreliableMessageReceived;
//			if (reliable)
//			{
//				type = NetEventType.ReliableMessageReceived;
//			}
//			this.Enqueue(type, userId, buffer);
//		}

//		private void InternalDisconnect(ConnectionId id)
//		{
//			if (this.mConnectionNetwork.ContainsKey(id))
//			{
//				this.Enqueue(NetEventType.Disconnected, id, null);
//				this.mConnectionNetwork.Remove(id);
//			}
//		}

//		private void InternalDisconnect(LocalNetwork ln)
//		{
//			this.InternalDisconnect(this.FindConnectionId(ln));
//		}

//		private void CleanupWreakReferences()
//		{
//			foreach (ConnectionId kvp in this.mConnectionNetwork.Keys.ToList<ConnectionId>())
//			{
//				if (this.mConnectionNetwork[kvp].Get() == null)
//				{
//					this.InternalDisconnect(kvp);
//				}
//			}
//		}

//		protected virtual void Dispose(bool disposing)
//		{
//			if (!this.disposedValue)
//			{
//				if (disposing)
//				{
//					this.Shutdown();
//				}
//				this.disposedValue = true;
//			}
//		}

//		public void Dispose()
//		{
//			this.Dispose(true);
//		}
//	}
//}
