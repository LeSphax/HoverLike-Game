using UnityEngine.SceneManagement;

public enum GUIPart
{
    ABILITY,
    CHAT,
    MENU,
}

public class Scenes
{

    public const string Lobby = "Lobby";
    public const string Room = "Room";
    public const string Main = "Main";

    public const short Any = -1;
    public const short LobbyIndex = 0;
    public const short MainIndex = 1;

    public static GUIPart CurrentSceneDefaultGUIPart()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case LobbyIndex:
                return GUIPart.CHAT;
            case MainIndex:
                return GUIPart.ABILITY;
            default:
                return GUIPart.ABILITY;
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
