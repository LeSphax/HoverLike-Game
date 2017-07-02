using System;

namespace Byn.Net
{
    public class BrowserWebRtcNetworkFactory : AWebRtcNetworkFactory
    {
        private bool disposedValue = false;


        public override IWebRtcNetwork CreateDefault(string websocketUrl, string[] iceUrls = null)
        {

            if (iceUrls == null)
            {
                iceUrls = new string[] { "stun:stun.services.mozilla.com" };
            }
            string iceUrlsJson = "\"" + iceUrls[0] + "\"";
            for (int i = 1; i < iceUrls.Length; i++)
            {
                iceUrlsJson += ", \"" + iceUrls[i] + "\"";
            }


            //string websocketUrl = "ws://localhost:12776";
            string conf;
            if (websocketUrl == null)
            {
                conf = "{ \"signaling\" :  { \"class\": \"LocalNetwork\", \"param\" : null}, \"iceServers\":[" + iceUrlsJson + "]}";

            }
            else
            {
                conf = "{ \"signaling\" :  { \"class\": \"WebsocketNetwork\", \"param\" : \"" + websocketUrl + "\"}, \"iceServers\":[" + iceUrlsJson + "]}";
            }

            return new BrowserWebRtcNetwork(conf);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //free managed
                }
                //free unmanaged

                disposedValue = true;
            }
        }
        public override void Dispose()
        {
            Dispose(true);
        }

        internal override void OnNetworkDestroyed(WebRtcNetwork network)
        {
            throw new NotImplementedException();
        }
    }
}
