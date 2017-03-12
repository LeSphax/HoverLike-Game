var AWebRtcPeer = function() {
    function e(e) {
        this.mState = WebRtcPeerState.Invalid;
        this.mRtcInternalState = WebRtcInternalState.None;
        this.mIncomingSignalingQueue = new Queue;
        this.mOutgoingSignalingQueue = new Queue;
        this.mDidSendRandomNumber = false;
        this.mRandomNumerSent = 0;
        this.mOfferOptions = {
            offerToReceiveAudio: 0,
            offerToReceiveVideo: 0
        };
        this.gConnectionConfig = {
            optional: [{
                DtlsSrtpKeyAgreement: true
            }]
        };
        this.SetupPeer(e);
        this.OnSetup();
        this.mState = WebRtcPeerState.Created
    }
    e.prototype.GetState = function() {
        return this.mState
    };
    e.prototype.SetupPeer = function(e) {
        var t = this;
        var n = {
            iceServers: [{
                urls: "stun:stun.services.mozilla.com"
            }]
        };
        n.iceServers = new Array;
        for (var i = 0; i < e.length; i++) {
            var o = {
                urls: e[0]
            };
            n.iceServers.push(o)
        }
        var r = window.RTCPeerConnection || window.mozRTCPeerConnection || window.webkitRTCPeerConnection;
        this.mPeer = new r(n, this.gConnectionConfig);
        this.mPeer.onicecandidate = function(e) {
            t.OnIceCandidate(e)
        };
        this.mPeer.oniceconnectionstatechange = function(e) {
            t.OnIceConnectionChange()
        };
        this.mPeer.onnegotiationneeded = function(e) {
            t.OnRenegotiationNeeded()
        };
        this.mPeer.onsignalingstatechange = function(e) {
            t.OnSignalingChange()
        }
    };
    e.prototype.DisposeInternal = function() {
        this.Cleanup()
    };
    e.prototype.Dispose = function() {
        if (this.mPeer != null) {
            this.DisposeInternal()
        }
    };
    e.prototype.Cleanup = function() {
        if (this.mState == WebRtcPeerState.Closed || this.mState == WebRtcPeerState.Closing) {
            return
        }
        this.mState = WebRtcPeerState.Closing;
        this.OnCleanup();
        if (this.mPeer != null) this.mPeer.close();
        this.mState = WebRtcPeerState.Closed
    };
    e.prototype.Update = function() {
        if (this.mState != WebRtcPeerState.Closed && this.mState != WebRtcPeerState.Closing && this.mState != WebRtcPeerState.SignalingFailed) this.UpdateState();
        if (this.mState == WebRtcPeerState.Signaling || this.mState == WebRtcPeerState.Created) this.HandleIncomingSignaling()
    };
    e.prototype.UpdateState = function() {
        if (this.mRtcInternalState == WebRtcInternalState.Closed) {
            this.Cleanup()
        } else if (this.mRtcInternalState == WebRtcInternalState.SignalingFailed) {
            this.mState = WebRtcPeerState.SignalingFailed
        } else if (this.mRtcInternalState == WebRtcInternalState.Connected) {
            this.mState = WebRtcPeerState.Connected
        }
    };
    e.prototype.HandleIncomingSignaling = function() {
        while (this.mIncomingSignalingQueue.Count() > 0) {
            var e = this.mIncomingSignalingQueue.Dequeue();
            var t = Helper.tryParseInt(e);
            if (t != null) {
                if (this.mDidSendRandomNumber) {
                    if (t < this.mRandomNumerSent) {
                        SLog.L("Signaling negotiation complete. Starting signaling.");
                        this.StartSignaling()
                    } else if (t == this.mRandomNumerSent) {
                        this.NegotiateSignaling()
                    } else {
                        SLog.L("Signaling negotiation complete. Waiting for signaling.")
                    }
                } else {}
            } else {
                var n = JSON.parse(e);
                if (n.sdp) {
                    var i = new RTCSessionDescription(n);
                    if (i.type == "offer") {
                        this.CreateAnswer(i)
                    } else {
                        this.RecAnswer(i)
                    }
                } else {
                    var o = new RTCIceCandidate(n);
                    if (o != null) {
                        this.mPeer.addIceCandidate(o, function() {}, function(e) {
                            Debug.LogError(e)
                        })
                    }
                }
            }
        }
    };
    e.prototype.AddSignalingMessage = function(e) {
        this.mIncomingSignalingQueue.Enqueue(e)
    };
    e.prototype.DequeueSignalingMessage = function(e) {
        {
            if (this.mOutgoingSignalingQueue.Count() > 0) {
                e.val = this.mOutgoingSignalingQueue.Dequeue();
                return true
            } else {
                e.val = null;
                return false
            }
        }
    };
    e.prototype.EnqueueOutgoing = function(e) {
        {
            this.mOutgoingSignalingQueue.Enqueue(e)
        }
    };
    e.prototype.StartSignaling = function() {
        this.OnStartSignaling();
        this.CreateOffer()
    };
    e.prototype.NegotiateSignaling = function() {
        var e = Random.getRandomInt(0, 2147483647);
        this.mRandomNumerSent = e;
        this.mDidSendRandomNumber = true;
        this.EnqueueOutgoing("" + e)
    };
    e.prototype.CreateOffer = function() {
        var e = this;
        Debug.Log("CreateOffer");
        this.mPeer.createOffer(function(t) {
            var n = JSON.stringify(t);
            e.mPeer.setLocalDescription(t, function() {
                e.RtcSetSignalingStarted();
                e.EnqueueOutgoing(n)
            }, function(t) {
                Debug.LogError(t);
                e.RtcSetSignalingFailed()
            })
        }, function(t) {
            Debug.LogError(t);
            e.RtcSetSignalingFailed()
        }, this.mOfferOptions)
    };
    e.prototype.CreateAnswer = function(e) {
        var t = this;
        Debug.Log("CreateAnswer");
        this.mPeer.setRemoteDescription(e, function() {
            t.mPeer.createAnswer(function(e) {
                var n = JSON.stringify(e);
                t.mPeer.setLocalDescription(e, function() {
                    t.RtcSetSignalingStarted();
                    t.EnqueueOutgoing(n)
                }, function(e) {
                    Debug.LogError(e);
                    t.RtcSetSignalingFailed()
                })
            }, function(e) {
                Debug.LogError(e);
                t.RtcSetSignalingFailed()
            })
        }, function(e) {
            Debug.LogError(e);
            t.RtcSetSignalingFailed()
        })
    };
    e.prototype.RecAnswer = function(e) {
        var t = this;
        Debug.Log("RecAnswer");
        this.mPeer.setRemoteDescription(e, function() {}, function(e) {
            Debug.LogError(e);
            t.RtcSetSignalingFailed()
        })
    };
    e.prototype.RtcSetSignalingStarted = function() {
        if (this.mRtcInternalState == WebRtcInternalState.None) {
            this.mRtcInternalState = WebRtcInternalState.Signaling
        }
    };
    e.prototype.RtcSetSignalingFailed = function() {
        this.mRtcInternalState = WebRtcInternalState.SignalingFailed
    };
    e.prototype.RtcSetConnected = function() {
        if (this.mRtcInternalState == WebRtcInternalState.Signaling) this.mRtcInternalState = WebRtcInternalState.Connected
    };
    e.prototype.RtcSetClosed = function() {
        if (this.mRtcInternalState == WebRtcInternalState.Connected) this.mRtcInternalState = WebRtcInternalState.Closed
    };
    e.prototype.OnIceCandidate = function(e) {
        if (e && e.candidate) {
            var t = e.candidate;
            var n = JSON.stringify(t);
            this.EnqueueOutgoing(n)
        }
    };
    e.prototype.OnIceConnectionChange = function() {
        Debug.Log(this.mPeer.iceConnectionState);
        if (this.mPeer.iceConnectionState == "failed") {
            this.mState = WebRtcPeerState.SignalingFailed
        }
    };
    e.prototype.OnIceGatheringChange = function() {
        Debug.Log(this.mPeer.iceGatheringState)
    };
    e.prototype.OnRenegotiationNeeded = function() {};
    e.prototype.OnSignalingChange = function() {
        Debug.Log(this.mPeer.signalingState);
        if (this.mPeer.signalingState == "closed") {
            this.RtcSetClosed()
        }
    };
    return e
}();