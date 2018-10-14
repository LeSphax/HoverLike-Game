using UnityEngine.SceneManagement;
using Utilities;

public enum GUIPart
{
    ABILITY,
    CHAT,
    MENU,
}

public class Scenes
{

    public static Map<short, string> indexToSceneName;
    public static Map<short, string> IndexToSceneName
    {

        get
        {
            if (indexToSceneName == null)
            {
                indexToSceneName = new Map<short, string>();
                indexToSceneName.Add(0, Lobby);
                indexToSceneName.Add(1, Main);
                indexToSceneName.Add(2, ML);
            }
            return indexToSceneName;
        }
    }


    public const string Lobby = "Lobby";
    public const string Room = "Room";
    public const string Main = "Main";
    public const string ML = "MLTraining";

    public const short Any = -1;
    public const short LobbyIndex = 0;
    public const short MainIndex = 1;
    public const short MLIndex = 2;

    public static GUIPart CurrentSceneDefaultGUIPart()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case Lobby:
                return GUIPart.CHAT;
            case Main:
                return GUIPart.ABILITY;
            case ML:
                return GUIPart.ABILITY;
            default:
                return GUIPart.ABILITY;
        }
    }


    public static bool IsCurrentScene(int sceneIndex)
    {
        if (currentSceneId == MLIndex)
        {
            return sceneIndex == MainIndex;
        }
        return currentSceneId == sceneIndex;
    }

    public static short currentSceneId
    {
        get
        {
            return  IndexToSceneName.Reverse[SceneManager.GetActiveScene().name];
        }
    }

}
