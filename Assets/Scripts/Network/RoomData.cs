using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomData
{
    private const char ROOM_DATA_SEPARATOR_CHAR = '|';

    public string name;
    public int nbPlayers;
    public bool gameStarted;
    public bool hasPassword;

    public RoomData(string data)
    {
        string[] split = data.Split(ROOM_DATA_SEPARATOR_CHAR);
        name = split[0];
        nbPlayers = int.Parse(split[1]);
        gameStarted = int.Parse(split[2]) == 1;
        hasPassword = int.Parse(split[3]) == 1;
    }


    public static List<RoomData> GetRoomData(string[] data)
    {
        List<RoomData> result = new List<RoomData>();
        for (int i = 1; i < data.Length; i++)
        {
            result.Add(new RoomData(data[i]));
        }
        return result;
    }
}
