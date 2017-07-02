using System.Collections.Generic;

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

    public RoomData(int nbPlayers, bool gameStarted, bool hasPassword)
    {
        this.nbPlayers = nbPlayers;
        this.gameStarted = gameStarted;
        this.hasPassword = hasPassword;
    }

    public string ToNetworkEntity()
    {
        return nbPlayers.ToString() + ROOM_DATA_SEPARATOR_CHAR + (gameStarted ? 1 : 0).ToString() + ROOM_DATA_SEPARATOR_CHAR + (hasPassword ? 1 : 0).ToString();
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
