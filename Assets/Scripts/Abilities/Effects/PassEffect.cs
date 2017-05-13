using AbilitiesManagement;
using Byn.Net;
using PlayerManagement;
using UnityEngine;
using UnityEngine.Assertions;

public class PassEffect : AbilityEffect
{
    public override void ApplyOnTarget(params object[] parameters)
    {
        ConnectionId targetId = (ConnectionId)parameters[0];
        if (targetId != Players.INVALID_PLAYER_ID)
            Players.MyPlayer.controller.View.RPC("Pass", RPCTargets.Server, targetId);
    }



}

public class PassPersistentEffect : PersistentEffect
{
    Transform target;
    const float SPEED = 70;
    const float VERTICAL_MAXIMUM = 20;
    AnimationCurve curve;

    public PassPersistentEffect(AbilitiesManager manager, ConnectionId id, AnimationCurve curve) : base(manager)
    {
        MyComponents.BallState.DetachBall();
        MyComponents.BallState.UnCatchable = true;
        MyComponents.BallState.PassTarget = id;

        this.curve = curve;
        target = Players.players[id].controller.transform;

        duration = Mathf.Infinity;
        MyComponents.BallState.UncatchableChanged += EndPassIfBallCaught;
    }

    protected override void Apply(float dt)
    {
        //On ramène la position de la balle sur la droite entre les deux joueurs qui se font la passe.
        Vector3 trajectoryPlaneNormal = Vector3.Normalize(Vector3.Cross(target.position - manager.transform.position, Vector3.up));
        Vector3 proj_ballPositionOnTrajectoryPlane = MyComponents.BallState.transform.position - Vector3.Dot(MyComponents.BallState.transform.position - target.position, trajectoryPlaneNormal) * trajectoryPlaneNormal;
        Vector3 proj_ballPositionOnTrajectoryLine = new Vector3(proj_ballPositionOnTrajectoryPlane.x, manager.transform.position.y, proj_ballPositionOnTrajectoryPlane.z);

        Vector3 newPositionOnLine = Vector3.MoveTowards(proj_ballPositionOnTrajectoryLine, target.position, SPEED * dt);

        //Proportion du mouvement horizontal effectué
        float H_completion = Vector3.Distance(manager.transform.position, newPositionOnLine) / Vector3.Distance(manager.transform.position, target.position);
        Debug.Log(H_completion);
        // A partir de la proportion du mouvement horizontal, on calcule la position verticale
        float yPos = curve.Evaluate(H_completion) * VERTICAL_MAXIMUM;

        Vector3 newHorizontalPosition = new Vector3(newPositionOnLine.x, 0, newPositionOnLine.z);
        Vector3 newPositionWithoutVerticalChange = newHorizontalPosition + Vector3.up * MyComponents.BallState.transform.position.y;
        Vector3 targetPosition = newHorizontalPosition + Vector3.up * yPos;
        //Debug.Log(trajectoryPlaneNormal + "   " + proj_ballPositionOnTrajectoryLine + "   " + proj_ballPositionOnTrajectoryPlane + "   " + newPositionOnLine + "   " + newHorizontalPosition + "   " + newPositionWithoutVerticalChange + "    " + targetPosition);
        //Debug.Log(manager.transform.position + "    " + target.position + "    " + H_completion + "     " + yPos);

        MyComponents.BallState.transform.position = Vector3.MoveTowards(newPositionWithoutVerticalChange, targetPosition, SPEED * dt);
    }

    private void EndPassIfBallCaught(bool newUncatchable)
    {
        Assert.IsFalse(newUncatchable);
        if (newUncatchable == false)
        {
            MyComponents.BallState.UncatchableChanged -= EndPassIfBallCaught;
            StopEffect();
            DestroyEffect();
        }
    }

    public override void StopEffect()
    {
        //MyComponents.BallState.SetAttached(targetId);
        //MyComponents.BallState.UnCatchable = false;
        MyComponents.BallState.PassTarget = Players.INVALID_PLAYER_ID;
        //Collider[] colliders = Physics.OverlapSphere(targetPosition, DIAMETER_PASS_ZONE / 2, LayersGetter.PlayersMask());

        //float closestPlayerDistance = Mathf.Infinity;
        //int closestPlayerMatchingTeam = 0;
        //PlayerController closestPlayer = null;

        //foreach (Collider hit in colliders)
        //{
        //    GameObject go = hit.gameObject;
        //    PlayerController controller = GetParentController(go.transform);
        //    if (controller != null)
        //    {
        //        float distance = Vector3.Distance(targetPosition, controller.transform.position);
        //        int matchingTeam = controller.Player.Team == manager.controller.Player.Team ? 1 : 0;

        //        if (distance < closestPlayerDistance || closestPlayerMatchingTeam < matchingTeam)
        //        {
        //            closestPlayerDistance = distance;
        //            closestPlayerMatchingTeam = matchingTeam;
        //            closestPlayer = controller;
        //        }
        //    }
        //}
        //MyComponents.BallState.Uncatchable = false;

        //if (closestPlayer != null)
        //    MyComponents.BallState.SetAttached(closestPlayer.playerConnectionId);
    }

    //private PlayerController GetParentController(Transform t)
    //{
    //    PlayerController controller = t.GetComponent<PlayerController>();
    //    if (controller != null)
    //    {
    //        return controller;
    //    }
    //    else if (t.parent != null)
    //        return GetParentController(t.parent);
    //    else
    //        return null;
    //}
}