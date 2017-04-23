using UnityEngine.SceneManagement;

public class Scenes
{

    public const string Lobby = "Lobby";
    public const string Room = "Room";
    public const string Main = "Main";

    public const short LobbyIndex = 0;
    public const short RoomIndex = 1;
    public const short MainIndex = 2;

    public static SlideBallInputs.GUIPart CurrentSceneDefaultGUIPart()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case LobbyIndex:
            case RoomIndex:
                return SlideBallInputs.GUIPart.CHAT;
            case MainIndex:
                return SlideBallInputs.GUIPart.ABILITY;
            default:
                return SlideBallInputs.GUIPart.ABILITY;
        }
    }


    public static bool IsCurrentScene(int sceneBuildIndex)
    {
        return SceneManager.GetActiveScene().buildIndex == sceneBuildIndex;
    }

    public static short currentSceneId
    {
        get
        {
            return (short)SceneManager.GetActiveScene().buildIndex;
        }
    }

}
