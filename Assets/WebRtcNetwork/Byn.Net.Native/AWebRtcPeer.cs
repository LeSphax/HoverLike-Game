using Byn.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using WebRtcCSharp;

namespace Byn.Net.Native
{
    public abstract class AWebRtcPeer : IDisposable
    {
        public enum PeerState
        {
            Invalid,
            Created,
            Signaling,
            SignalingFailed,
            Connected,
            Closing,
            Closed
        }

        private enum WebRtcInternalState
        {
            None,
            Signaling,
            SignalingFailed,
            Connected,
            Closed
        }



        private NativeWebRtcNetworkFactory mFactory;

        private object mLock = new object();

        private PeerState mState;

        private WebRtcInternalState mRtcInternalState;

        protected RTCPeerConnection mPeer;

        protected PeerConnectionInterface.RTCOfferAnswerOptions mOfferOptions;

        private Queue<string> mIncomingSignalingQueue = new Queue<string>();

        private Queue<string> mOutgoingSignalingQueue = new Queue<string>();

        private bool mDidSendRandomNumber;

        private int mRandomNumberSent;

        public PeerState State
        {
            get
            {
                return this.mState;
            }
        }

        public AWebRtcPeer(string[] urls, NativeWebRtcNetworkFactory factory)
        {
            this.mFactory = factory;
            this.SetupPeer(urls);
            this.OnSetup();
            this.mState = PeerState.Created;
        }

        protected abstract void OnSetup();

        protected abstract void OnStartSignaling();

        protected abstract void OnCleanup();

        private void SetupPeer(string[] urls)
        {
            PeerConnectionInterface.RTCConfiguration config = new PeerConnectionInterface.RTCConfiguration();
            PeerConnectionInterface.IceServer server = new PeerConnectionInterface.IceServer();
            for (int i = 0; i < urls.Length; i++)
            {
                server.urls.Add(urls[i]);
            }
            config.servers.Add(server);
            this.mPeer = this.mFactory.NativeFactory.CreatePeerConnection(config);
            this.mPeer.OnIceCandidate = new Action<IceCandidateInterface>(this.OnIceCandidate);
            this.mPeer.OnIceConnectionChange = new Action<PeerConnectionInterface.IceConnectionState>(this.OnIceConnectionChange);
            this.mPeer.OnRenegotiationNeeded = new Action(this.OnRenegotiationNeeded);
            this.mPeer.OnSignalingChange = new Action<PeerConnectionInterface.SignalingState>(this.OnSignalingChange);
        }

        public void Dispose()
        {
            if (this.mPeer != null)
            {
                this.Cleanup();
            }
        }

        private void Cleanup()
        {
            if (this.mState == PeerState.Closed || this.mState == PeerState.Closing)
            {
                return;
            }
            this.mState = PeerState.Closing;
            this.OnCleanup();
            if (this.mPeer != null)
            {
                this.mPeer.Dispose();
            }
            this.mPeer = null;
            this.mState = PeerState.Closed;
        }

        public void Update()
        {
            if (this.mState != PeerState.Closed && this.mState != PeerState.Closing && this.mState != PeerState.SignalingFailed)
            {
                this.UpdateState();
            }
            if (this.mState == PeerState.Signaling || this.mState == PeerState.Created)
            {
                this.HandleIncomingSignaling();
            }
        }

        private void UpdateState()
        {
            if (this.mRtcInternalState == WebRtcInternalState.Closed)
            {
                this.Cleanup();
                return;
            }
            if (this.mRtcInternalState == WebRtcInternalState.SignalingFailed)
            {
                this.mState = PeerState.SignalingFailed;
                return;
            }
            if (this.mRtcInternalState == WebRtcInternalState.Connected)
            {
                this.mState = PeerState.Connected;
            }
        }

        public void StartSignaling()
        {
            this.OnStartSignaling();
            this.CreateOffer();
        }

        public void NegotiateSignaling()
        {
            int nb = new System.Random(Guid.NewGuid().GetHashCode()).Next();
            this.mRandomNumberSent = nb;
            this.mDidSendRandomNumber = true;
            this.EnqueueOutgoing(string.Concat(nb));
        }

        public void AddSignalingMessage(string msg)
        {
            this.mIncomingSignalingQueue.Enqueue(msg);
        }

        public bool DequeueSignalingMessage(out string msg)
        {
            msg = null;
            object obj = this.mLock;
            bool result;
            lock (obj)
            {
                if (this.mOutgoingSignalingQueue.Count > 0)
                {
                    msg = this.mOutgoingSignalingQueue.Dequeue();
                    result = true;
                }
                else
                {
                    msg = null;
                    result = false;
                }
            }
            return result;
        }

        private void HandleIncomingSignaling()
        {
            while (this.mIncomingSignalingQueue.Count > 0)
            {
                string msg = this.mIncomingSignalingQueue.Dequeue();
                int randomNumber;
                if (int.TryParse(msg, out randomNumber))
                {
                    if (this.mDidSendRandomNumber)
                    {
                        if (randomNumber < this.mRandomNumberSent)
                        {
                            SLog.L("Signaling negotiation complete. Starting signaling.", new string[0]);
                            this.StartSignaling();
                        }
                        else if (randomNumber == this.mRandomNumberSent)
                        {
                            this.NegotiateSignaling();
                        }
                        else
                        {
                            SLog.L("Signaling negotiation complete. Waiting for signaling.", new string[0]);
                        }
                    }
                }
                else if (WebRtcWrap.IsIceCandidateJson(msg))
                {
                    IceCandidateInterface ice = WebRtcWrap.ParseJsonIceCandidate(msg);
                    if (ice != null)
                    {
                        this.mPeer.AddIceCandidate(ice);
                    }
                    else
                    {
                        SLog.LW("Invalid ice message received", new string[0]);
                    }
                }
                else if (WebRtcWrap.IsSessionDescriptionJson(msg))
                {
                    RTCSessionDescription sdi = RTCSessionDescription.FromJson(msg);
                    if (sdi != null)
                    {
                        if (sdi.type() == SessionDescriptionInterface.kOffer)
                        {
                            this.CreateAnswer(sdi);
                        }
                        else
                        {
                            this.RecAnswer(sdi);
                        }
                    }
                    else
                    {
                        SLog.LW("Invalid session description message received", new string[0]);
                    }
                }
                else
                {
                    SLog.LW("Invalid signaling message received", new string[0]);
                }
            }
        }

        private void EnqueueOutgoing(string msg)
        {
            object obj = this.mLock;
            lock (obj)
            {
                SLog.L("Signaling message " + msg, new string[0]);
                this.mOutgoingSignalingQueue.Enqueue(msg);
            }
        }

        private void CreateOffer()
        {
            this.mState = PeerState.Signaling;
            SLog.L("CreateOffer", new string[0]);
            this.mPeer.CreateOffer(delegate (RTCSessionDescription desc)
            {
                string msg = desc.ToJson();
                this.mPeer.SetLocalDescription(desc, delegate
                {
                    this.RtcSetSignalingStarted();
                    this.EnqueueOutgoing(msg);
                }, delegate (string error)
                {
                    SLog.LE(error, new string[0]);
                    this.RtcSetSignalingFailed();
                });
            }, delegate (string error)
            {
                SLog.LE(error, new string[0]);
                this.RtcSetSignalingFailed();
            }, this.mOfferOptions);
        }

        private void CreateAnswer(RTCSessionDescription offer)
        {
            this.mState = PeerState.Signaling;
            SLog.L("CreateAnswer", new string[0]);
            this.mPeer.SetRemoteDescription(offer, delegate
            {
                this.mPeer.CreateAnswer(delegate (RTCSessionDescription desc)
                {
                    string msg = desc.ToJson();
                    this.mPeer.SetLocalDescription(desc, delegate
                    {
                        this.RtcSetSignalingStarted();
                        this.EnqueueOutgoing(msg);
                    }, delegate (string error)
                    {
                        SLog.LE(error, new string[0]);
                        this.RtcSetSignalingFailed();
                    });
                }, delegate (string error)
                {
                    SLog.LE(error, new string[0]);
                    this.RtcSetSignalingFailed();
                }, null);
            }, delegate (string error)
            {
                SLog.LE(error, new string[0]);
                this.RtcSetSignalingFailed();
            });
        }

        private void RecAnswer(RTCSessionDescription answer)
        {
            SLog.L("RecAnswer", new string[0]);
            RTCPeerConnection rtcPeerConnection = this.mPeer;
            rtcPeerConnection.SetRemoteDescription(answer, new Action(delegate () { Debug.Log("Remote Description success"); }), delegate (string error)
             {
                 SLog.LE(error, new string[0]);
                 this.RtcSetSignalingFailed();
             });
        }

        private void RtcSetSignalingStarted()
        {
            if (this.mRtcInternalState == WebRtcInternalState.None)
            {
                this.mRtcInternalState = WebRtcInternalState.Signaling;
            }
        }

        protected void RtcSetSignalingFailed()
        {
            Debug.LogError("AWebRtcPeer RtcSignalingFailed");
            this.mRtcInternalState = WebRtcInternalState.SignalingFailed;
        }

        protected void RtcSetConnected()
        {
            if (this.mRtcInternalState == WebRtcInternalState.Signaling)
            {
                this.mRtcInternalState = WebRtcInternalState.Connected;
            }
        }

        protected void RtcSetClosed()
        {
            if (this.mRtcInternalState == WebRtcInternalState.Connected)
            {
                this.mRtcInternalState = WebRtcInternalState.Closed;
            }
        }

        private void OnIceCandidate(IceCandidateInterface candidate)
        {
            string msg = WebRtcWrap.ToJson(candidate);
            this.EnqueueOutgoing(msg);
        }

        private void OnIceConnectionChange(PeerConnectionInterface.IceConnectionState new_state)
        {
            SLog.L(new_state, new string[0]);
            if (new_state == PeerConnectionInterface.IceConnectionState.kIceConnectionFailed)
            {
                this.mState = PeerState.SignalingFailed;
            }
        }

        private void OnIceGatheringChange(PeerConnectionInterface.IceGatheringState new_state)
        {
            SLog.L(new_state, new string[0]);
        }

        private void OnRenegotiationNeeded()
        {
        }

        private void OnSignalingChange(PeerConnectionInterface.SignalingState new_state)
        {
            SLog.L(new_state, new string[0]);
            if (new_state == PeerConnectionInterface.SignalingState.kClosed)
            {
                this.RtcSetClosed();
            }
        }
    }
}
