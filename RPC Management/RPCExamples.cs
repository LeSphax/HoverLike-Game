
//Example 1 

//As long as a component inherits SLideBall.MonoBehaviour, we can call its RPC like this.
public class AbilitiesManager : SlideBall.Monobehaviour
{

    //...
    [MyRPC]
    //This method is called server side to initiate a shot.
    private void Shoot(Vector3 target, float power)
    {
        if (CanUseAbility())
            controller.ballController.ThrowBall(target, power);
    }
    // ...
}

//Client side, after the user pressed the shoot button and selected his target.
AbilitiesManager abilitiesManager;

abilitiesManager.View.RPC("Shoot", RPCTargets.Server, position, GetComponent<PowerBar>().powerValue);


//Example 2
public class MatchManager : SlideBall.Monobehaviour
{
    // ...
    [MyRPC]
    //We use this method to synchronize the state of the object across the network
    private void SetState(State newState)
    {
        MyState = newState;
    }
    //...
}

// The server will call this each time the state has changed.
View.RPC("SetState", RPCTargets.Others, value);


//Example with RPCSenderId

    public class PlayerInfo : PlayerView
{
    //...

    void Start()
    {
        if (View.isMine)
            if (Player.Team != Team.NONE)
                SetInitialTeam(Player.Team);
            else
                //Asks the server to set the players team
                View.RPC("GetInitialTeam", RPCTargets.Server, null);
    }

    [MyRPC]
    //Calling a parameter RPCSenderId means it is a special parameter that doesn't need to be set when the RPC is called.
    //This information is alreay present in all network messages so it doesn't need to be in the RPC parameters.
    public void GetInitialTeam(ConnectionId RPCSenderId)
    {
        Assert.IsTrue(MyComponents.NetworkManagement.isServer);
        Team team;
        if (Players.GetPlayersInTeam(Team.BLUE).Count <= Players.GetPlayersInTeam(Team.RED).Count)
            team = Team.BLUE;
        else
            team = Team.RED;
        //Send back the team to the client
        View.RPC("SetInitialTeam", RPCSenderId, team);
        Players.players[RPCSenderId].Team = team;
    }

    [MyRPC]
    public void SetInitialTeam(Team team)
    {
        Players.MyPlayer.Team = team;
    }
    //...
}