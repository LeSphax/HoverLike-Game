var WebRtcNetwork = function () {
    function e(signalingServerConnection, peerNetwork) {
        this.isDisposed = false;
        this.signalingServerConnection = signalingServerConnection;
        this.peerNetwork = peerNetwork;
        this.signalingServerConnection.SetPeerNetwork(peerNetwork);
    };

    e.prototype.UpdateNetwork = function () {
        this.peerNetwork.peerSignalingManager.CheckSignalingState();
        this.signalingServerConnection.UpdateSignalingNetwork();
        this.peerNetwork.UpdatePeers();
    }

    e.prototype.Dispose = function () {
        if (!this.isDisposed) {
            this.Shutdown();
            this.isDisposed = true;
        }
    }
    e.prototype.Shutdown = function () {
        this.signalingServerConnection.Dispose();
        this.peerNetwork.Dispose();
    }

    e.prototype.Flush = function () {
        this.peerNetwork.Flush();
        this.signalingServerConnection.Flush();
    }
    return e;
}();
