var MyPlugin = {

    GetRoomName: function()
    {
        var vars = {};
        var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi,    
            function(m,key,value) {
                vars[key] = value;
            });

        var captured = vars["RoomName"];
        var result = captured ? captured : 'NoRoomName';
        var buffer = _malloc(lengthBytesUTF8(result) + 1);
        writeStringToMemory(result, buffer);
        console.log("RoomName " + window.location + "   " + result);
        return buffer;
    },

    UnityReady : function(){
        UnityIsReady();
    },

    ShowLink: function(roomName){
        var str_roomName = Pointer_stringify(roomName);
        window.prompt("Copy to clipboard: Ctrl+C, Enter",window.location+"?RoomName=" + str_roomName);
    },
};

mergeInto(LibraryManager.library, MyPlugin);