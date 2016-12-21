using UnityEngine;

public enum Team : byte
{
    NONE = 100,
    BLUE = 0,
    RED = 1,
}

public static class Teams
{
    public static string GetTeamNameKey(Team team)
    {
        switch (team)
        {
            case Team.BLUE:
                return "Blue";
            case Team.RED:
                return "Red";
            default:
                Debug.LogError("This team doesn't have a name " + team);
                return "Blue";
        }
    }
}