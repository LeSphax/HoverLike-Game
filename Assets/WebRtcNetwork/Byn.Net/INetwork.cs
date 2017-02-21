using System;
using System.Collections.Generic;

namespace Byn.Net
{
	public interface INetwork : IDisposable
	{
		bool Dequeue(out NetworkEvent evt);

		bool Peek(out NetworkEvent evt);

		void Flush();

		void SendEvent(ConnectionId id, byte[] data, int offset, int length, bool reliable);

		void Disconnect(ConnectionId id);

		[Obsolete("Use Dispose instead and recreate a new network if needed.")]
		void Shutdown();

        Queue<NetworkEvent> UpdateNetwork();
	}
}
