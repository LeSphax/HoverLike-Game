using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomData
{
    private const char NB_PLAYERS_SEPARATOR_CHAR = '|';

    public string name;
    public int nbPlayers;

    public RoomData(string data)
    {
        string[] split = data.Split(NB_PLAYERS_SEPARATOR_CHAR);
        name = split[0];
        nbPlayers = int.Parse(split[1]);
    }


    public static List<RoomData> GetRoomData(string[] data)
    {
        List<RoomData> result = new List<RoomData>();
        for(int i =1; i<data.Length; i++)
        {
            result.Add(new RoomData(data[i]));
        }
        return result;
    }
}
