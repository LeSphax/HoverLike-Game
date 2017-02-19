using System;

namespace Byn.Net
{
    public interface IPeerNetwork : INetwork, IDisposable
	{
        void CheckSignalingState();
	}
}
