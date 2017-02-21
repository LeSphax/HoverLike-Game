using System;
using System.Collections.Generic;

namespace Byn.Net
{
    public interface IPeerNetwork : INetwork, IDisposable
	{
        Queue<NetworkEvent> PeerEvents { get; }

        void CheckSignalingState();
	}
}
