using System;

namespace Byn.Net
{
	public abstract class AWebRtcNetworkFactory : IDisposable
	{
        public abstract WebRtcNetwork CreateDefault(string websocketUrl, string[] urls = null);

        internal abstract void OnNetworkDestroyed(WebRtcNetwork network);

        public abstract void Dispose();
    }
}
