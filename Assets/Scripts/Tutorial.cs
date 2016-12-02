using PlayerManagement;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public GameObject LeftClickGif;
    public GameObject RightClickGif;
    public GameObject AbilitiesImage;
    public Text Explanation;
    public Text buttonContent;

    public enum State
    {
        RIGHT,
        LEFT,
        ABILITIES,
        CLOSED,
    }

    private State myState;
    private State MyState
    {
        set
        {
            myState = value;
            switch (value)
            {
                case State.RIGHT:
                    gameObject.SetActive(true);
                    LeftClickGif.SetActive(false);
                    RightClickGif.SetActive(true);
                    AbilitiesImage.SetActive(false);
                    Explanation.text = Language.Instance.texts["Tuto_RC"];
                    buttonContent.text = Language.Instance.texts["Button_Next"];
                    break;
                case State.LEFT:
                    gameObject.SetActive(true);
                    LeftClickGif.SetActive(true);
                    RightClickGif.SetActive(false);
                    AbilitiesImage.SetActive(false);
                    Explanation.text = Language.Instance.texts["Tuto_LC"];
                    buttonContent.text = Language.Instance.texts["Button_Next"];
                    break;
                case State.ABILITIES:
                    gameObject.SetActive(true);
                    LeftClickGif.SetActive(false);
                    RightClickGif.SetActive(false);
                    AbilitiesImage.SetActive(true);
                    Explanation.text = Language.Instance.texts["Tuto_Abilities"];
                    buttonContent.text = Language.Instance.texts["Button_Close"];
                    break;
                case State.CLOSED:
                    gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }
    }

    public void Start()
    {
        if (JavascriptAPI.isFirstGame)
            Reset();
        else
        {
            MyState = State.CLOSED;
        }
    }

    public void Reset()
    {
        MyState = State.RIGHT;
    }

    public void SetState(State state)
    {
        MyState = state;
    }

    public void ButtonPressed()
    {
        switch (myState)
        {
            case State.RIGHT:
                MyState = State.LEFT;
                break;
            case State.LEFT:
                MyState = State.ABILITIES;
                break;
            case State.ABILITIES:
                MyState = State.CLOSED;
                break;
            case State.CLOSED:
                Debug.LogError("This shouldn't happen");
                break;
            default:
                break;
        }
    }



    //public PlayerController player;
    //public GameObject ball;

    //protected void Awake()
    //{
    //    gameObject.tag = Tags.Tutorial;

    //}

    //private void Start()
    //{
    //    this.InstantiateRessource("Terrain", transform);

    //    player = this.InstantiateRessource("MyPlayer", transform).GetComponent<PlayerController>();
    //    player.InitPlayer(Players.myPlayerId);
    //    Destroy(player.GetComponent<RemovePhysicsIfClient>());

    //    //ball = this.InstantiateRessource("Ball", transform);

    //    player.GetComponent<MyNetworkView>().isLocal = true;
    //    //ball.GetComponent<MyNetworkView>().isLocal = true;

    //}

}
