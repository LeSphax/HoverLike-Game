/* 
 * Copyright (C) 2015 Christoph Kutza
 * 
 * Please refer to the LICENSE file for license information
 */
 
var UnityWebRtcNetwork =
{
	UnityWebRtcNetworkIsAvailable:function()
    {
		if(typeof CAPIWebRtcNetworkIsAvailable === 'function')
		{
			return CAPIWebRtcNetworkIsAvailable();
		}
		return false;
    },
	UnityWebRtcNetworkCreate:function(lConfiguration)
	{
		return CAPIWebRtcNetworkCreate(Pointer_stringify(lConfiguration));
	},
	UnityWebRtcNetworkRelease:function(lIndex)
	{
		CAPIWebRtcNetworkRelease(lIndex);
	},
	UnityWebRtcNetworkUpdateNetwork:function(lIndex)
	{
		CAPIWebRtcNetworkUpdateNetwork(lIndex);
	},
	UnityWebRtcNetworkFlush:function(lIndex)
	{
		CAPIWebRtcNetworkFlush(lIndex);
	},
	UnityWebRtcNetworkConnectToServer:function(lIndex)
	{
		return CAPIWebRtcNetworkConnectToServer(lIndex);
	},
	UnityWebRtcNetworkConnectToRoom:function(lIndex, lRoom)
	{
		return CAPIWebRtcNetworkConnectToRoom(lIndex, Pointer_stringify(lRoom));
	},
	UnityWebRtcNetworkCreateRoom:function(lIndex, lRoom)
	{
		CAPIWebRtcNetworkCreateRoom(lIndex, Pointer_stringify(lRoom));
	},
	UnityWebRtcNetworkLeaveRoom:function(lIndex)
	{
		CAPIWebRtcNetworkLeaveRoom(lIndex);
	},
	UnityWebRtcNetworkDisconnectFromServer:function(lIndex)
	{
		CAPIWebRtcNetworkDisconnectFromServer(lIndex);
	},
	UnityWebRtcNetworkDisconnectFromPeer:function(lIndex, lConnectionId)
	{
		CAPIWebRtcNetworkDisconnectFromPeer(lIndex, lConnectionId);
	},
	UnityWebRtcNetworkSendPeerData:function(lIndex, lConnectionId, lUint8ArrayDataPtr, lUint8ArrayDataOffset, lUint8ArrayDataLength, lReliable)
	{
		var sndReliable = true;
		if(lReliable == false || lReliable == 0 || lReliable == "false" || lReliable == "False")
			sndReliable = false;
		CAPIWebRtcNetworkSendPeerDataEm(lIndex, lConnectionId, HEAPU8, lUint8ArrayDataPtr + lUint8ArrayDataOffset, lUint8ArrayDataLength, sndReliable);
	},
	UnityWebRtcNetworkSendSignalingData:function(lIndex, lConnectionId, lTypeInt, lContent)
	{
		CAPIWebRtcNetworkSendSignalingDataEm(lIndex, lConnectionId, lTypeInt, Pointer_stringify(lContent));
	},
	UnityWebRtcNetworkShutdown:function(lIndex)
	{
		CAPIWebRtcNetworkShutdown(lIndex);
	},
	UnityWebRtcNetworkPeekEventDataLength:function(Signaling, lIndex)
	{
		return CAPIWebRtcNetworkPeekEventDataLength(Signaling, lIndex);
	},
	UnityWebRtcNetworkDequeue:function(Signaling, lIndex, lTypeIntArrayPtr, lConidIntArrayPtr, lUint8ArrayDataPtr, lUint8ArrayDataOffset, lUint8ArrayDataLength, lDataLenIntArrayPtr )
	{
		var val = CAPIWebRtcNetworkDequeueEm(Signaling, lIndex, HEAP32, lTypeIntArrayPtr >> 2, HEAP32, lConidIntArrayPtr >> 2, HEAPU8, lUint8ArrayDataPtr + lUint8ArrayDataOffset, lUint8ArrayDataLength, HEAP32, lDataLenIntArrayPtr >> 2);
		return val;
	},
	UnityWebRtcNetworkPeek:function(Signaling, lIndex, lTypeIntArrayPtr, lConidIntArrayPtr, lUint8ArrayDataPtr, lUint8ArrayDataOffset, lUint8ArrayDataLength, lDataLenIntArrayPtr )
	{
		var val = CAPIWebRtcNetworkPeekEm(Signaling, lIndex, HEAP32, lTypeIntArrayPtr >> 2, HEAP32, lConidIntArrayPtr >> 2, HEAPU8, lUint8ArrayDataPtr + lUint8ArrayDataOffset, lUint8ArrayDataLength, HEAP32, lDataLenIntArrayPtr >> 2);
		return val;
	}
};

mergeInto(LibraryManager.library, UnityWebRtcNetwork);