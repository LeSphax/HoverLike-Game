using Byn.Net;
using Byn.Net.Native;
using System;
using System.Collections.Generic;

public class WebRtcNetwork : IDisposable
{
    private bool mIsDisposed = false;

    private AWebRtcNetworkFactory mFactory;

    public IServerConnection serverConnection;
    public IPeerNetwork peerNetwork;

    private NetworkUpdateResult result;

    public WebRtcNetwork(IServerConnection serverConnection, IPeerNetwork peerNetwork, AWebRtcNetworkFactory factory)
    {
        this.serverConnection = serverConnection;
        this.peerNetwork = peerNetwork;
        this.mFactory = factory;
    }

    public void Dispose()
    {
        if (!this.mIsDisposed)
        {
            this.Shutdown();
            this.mFactory.OnNetworkDestroyed(this);
            this.mIsDisposed = true;
        }
    }

    public NetworkUpdateResult UpdateNetwork()
    {
        peerNetwork.CheckSignalingState();
        result.signalingEvents = serverConnection.UpdateNetwork();
        result.peerEvents = peerNetwork.UpdateNetwork();
        return result;
    }

    private void Shutdown()
    {
        serverConnection.Dispose();
        peerNetwork.Dispose();
    }

    internal void Flush()
    {
        peerNetwork.Flush();
        serverConnection.Flush();
    }
}

public struct NetworkUpdateResult
{
    public Queue<NetworkEvent> signalingEvents;
    public Queue<NetworkEvent> peerEvents;
}
