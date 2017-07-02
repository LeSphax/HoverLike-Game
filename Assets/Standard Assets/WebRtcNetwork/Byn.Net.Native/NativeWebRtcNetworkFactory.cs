using System;
using System.Collections.Generic;
using UnityEngine;
using WebRtcCSharp;

namespace Byn.Net.Native
{
    public class NativeWebRtcNetworkFactory : AWebRtcNetworkFactory, IDisposable
    {
        private RTCPeerConnectionFactory mFactory;

        private List<WebRtcNetwork> mCreatedNetworks = new List<WebRtcNetwork>();

        private bool disposedValue;

        public RTCPeerConnectionFactory NativeFactory
        {
            get
            {
                return this.mFactory;
            }
        }

        public NativeWebRtcNetworkFactory()
        {
            this.mFactory = new RTCPeerConnectionFactory();
        }

        public bool Initialize(bool multithreading = false)
        {
            return this.mFactory.IsInitialized() || this.mFactory.Initialize(multithreading);
        }

        public override IWebRtcNetwork CreateDefault(string urlOrConfig, string[] urls = null)
        {
            IServerConnection signalingNetwork;
            if (string.IsNullOrEmpty(urlOrConfig))
            {
                //signalingNetwork = new LocalNetwork();
                Debug.LogError("Can't connect to the signaling server : The url is null or empty");
                return null;
            }
            else
            {
                signalingNetwork = new ServerConnection(urlOrConfig);
            }
            WebRtcNetwork network = new WebRtcNetwork(signalingNetwork, Create(signalingNetwork, urls), this);
            this.mCreatedNetworks.Add(network);

            return network;
        }

        public IPeerNetwork Create(IServerConnection signalingNetwork, string[] urls = null)
        {
            if (urls == null)
            {
                urls = DefaultValues.DefaultIceServers;
            }
            if (!this.mFactory.IsInitialized())
            {
                bool worked = this.Initialize(false);
                if (!worked)
                {
                    worked = this.Initialize(true);
                }
                if (!worked)
                {
                    return null;
                }
            }
            NativeWebRtcNetwork net = new NativeWebRtcNetwork(signalingNetwork, urls, this);
            return net;
        }

        internal override void OnNetworkDestroyed(WebRtcNetwork network)
        {
            this.mCreatedNetworks.Remove(network);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    WebRtcNetwork[] array = this.mCreatedNetworks.ToArray();
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i].Dispose();
                    }
                    this.mCreatedNetworks.Clear();
                    if (this.mFactory != null)
                    {
                        this.mFactory.Cleanup();
                        this.mFactory.KillThreads();
                        this.mFactory.Dispose();
                        this.mFactory = null;
                    }
                }
                this.disposedValue = true;
            }
        }

        public override void Dispose()
        {
            this.Dispose(true);
        }
    }
}
