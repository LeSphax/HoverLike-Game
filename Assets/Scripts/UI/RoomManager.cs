//using Navigation;
//using PlayerManagement;
//using UnityEngine;

//public class RoomManager : MonoBehaviour
//{
//    public TopPanel topPanel;
//    public GameObject[] teamPanels;
//    public GameObject StartButton;

//    public static int MaxNumberPlayers = 7;
//    public static string Password = "";


//    private static RoomManager instance;
//    public static RoomManager Instance
//    {
//        get
//        {
//            Assert.IsTrue(Scenes.IsCurrentScene(Scenes.RoomIndex));
//            if (instance == null)
//            {
//                instance = GameObject.FindGameObjectWithTag(Tags.Room).GetComponent<RoomManager>();
//            }
//            return instance;
//        }
//    }

//    public void OnEnable()
//    {
//        MyComponents.NetworkManagement.ReceivedAllBufferedMessages += CreateMyPlayerInfo;
//        topPanel.BackPressed += GoBack;
//        MyComponents.NetworkManagement.RoomClosed += GoBack;

//    }

//    public void OnDisable()
//    {
//        if (MyComponents.NetworkManagement != null)
//            MyComponents.NetworkManagement.ReceivedAllBufferedMessages -= CreateMyPlayerInfo;
//        topPanel.BackPressed -= GoBack;
//        MyComponents.NetworkManagement.RoomClosed -= GoBack;
//    }

//    private void Start()
//    {
//        topPanel.Status = "In Game Room";
//        topPanel.RoomName = MyComponents.NetworkManagement.RoomName;
//        if (EditorVariables.StartGameImmediately)
//        {
//            InvokeRepeating("CheckStartGame", 0f, 0.2f);
//        }
//    }

//    void CheckStartGame()
//    {
//        if (MyComponents.NetworkManagement.IsServer && (MyComponents.NetworkManagement.GetNumberPlayers() == EditorVariables.NumberPlayersToStartGame || EditorVariables.NumberPlayersToStartGame == 1))
//        {
//            Invoke("StartGame", 2f);
//            CancelInvoke("CheckStartGame");
//        }
//    }

//    public void StartGame()
//    {
//        MyComponents.GameInitialization.StartGame();
//    }

//    public void PutPlayerInTeam(PlayerInfo player, Team team)
//    {
//        player.transform.SetParent(teamPanels[(int)team].transform, false);
//    }

//    public void ChangeTeam(int teamNumber)
//    {
//        Players.MyPlayer.Team = (Team)teamNumber;
//    }

//    public void CreateMyPlayerInfo()
//    {
//        if (Players.MyPlayer.gameobjectAvatar == null)
//            Players.MyPlayer.gameobjectAvatar = MyComponents.NetworkViewsManagement.Instantiate(Paths.PLAYER_INFO, Vector3.zero, Quaternion.identity, Players.MyPlayer.id);
//    }

//    public void Reset()
//    {
//        foreach (GameObject teamPanel in teamPanels)
//        {
//            foreach (Transform child in teamPanel.transform)
//            {
//                Destroy(child.gameObject);
//            }
//        }
//    }

//    public void GoBack()
//    {
//        MyComponents.ResetNetworkComponents();
//        NavigationManager.LoadScene(Scenes.Lobby);
//    }

//    protected void OnDestroy()
//    {
//        instance = null;
//    }

//}