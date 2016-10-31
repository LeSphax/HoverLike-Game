using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NickNamePanel : MonoBehaviour
{
    public InputField nicknameField;
    public static string nickname;

    public bool autoNickName;

    void Start()
    {
        if (autoNickName || MyGameObjects.LobbyManager.StartGameImmediately)
        {
            nicknameField.text = RandomString(5);
            SetNickname();
        }
    }

    public void SetNickname()
    {
        nickname = nicknameField.text;
        gameObject.SetActive(false);
    }

    private static System.Random random = new System.Random();
    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}