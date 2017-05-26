using Byn.Net;
using PlayerManagement;
using UnityEngine;
using UnityEngine.Assertions;

namespace Ball
{
    public class PassTrajectoryStrategy : BallTrajectoryStrategy
    {
        Transform passer;
        Transform target;
        const float SPEED = 70;
        const float VERTICAL_MAXIMUM = 20;
        AnimationCurve curve;

        public PassTrajectoryStrategy(ConnectionId passerId, ConnectionId targetId, AnimationCurve curve)
        {
            MyComponents.BallState.DetachBall();
            MyComponents.BallState.UnCatchable = true;
            MyComponents.BallState.PassTarget = targetId;
            MyComponents.BallState.transform.SetParent(null);

            this.curve = curve;
            passer = Players.players[passerId].controller.transform;
            target = Players.players[targetId].controller.transform;

            MyComponents.BallState.UncatchableChanged += EndPassIfBallCaught;
        }

        public override void MoveBall()
        {
            //On ramène la position de la balle sur la droite entre les deux joueurs qui se font la passe.
            Vector3 trajectoryPlaneNormal = Vector3.Normalize(Vector3.Cross(target.position - passer.position, Vector3.up));
            Vector3 proj_ballPositionOnTrajectoryPlane = MyComponents.BallState.transform.position - Vector3.Dot(MyComponents.BallState.transform.position - target.position, trajectoryPlaneNormal) * trajectoryPlaneNormal;
            Vector3 proj_ballPositionOnTrajectoryLine = new Vector3(proj_ballPositionOnTrajectoryPlane.x, passer.position.y, proj_ballPositionOnTrajectoryPlane.z);

            Vector3 newPositionOnLine = Vector3.MoveTowards(proj_ballPositionOnTrajectoryLine, target.position, SPEED * Time.fixedDeltaTime);

            //Proportion du mouvement horizontal effectué
            float H_completion = Vector3.Distance(passer.position, newPositionOnLine) / Vector3.Distance(passer.position, target.position);
            Debug.Log(H_completion);
            // A partir de la proportion du mouvement horizontal, on calcule la position verticale
            float yPos = curve.Evaluate(H_completion) * VERTICAL_MAXIMUM;

            Vector3 newHorizontalPosition = new Vector3(newPositionOnLine.x, 0, newPositionOnLine.z);
            Vector3 newPositionWithoutVerticalChange = newHorizontalPosition + Vector3.up * MyComponents.BallState.transform.position.y;
            Vector3 targetPosition = newHorizontalPosition + Vector3.up * yPos;
            //Debug.Log(trajectoryPlaneNormal + "   " + proj_ballPositionOnTrajectoryLine + "   " + proj_ballPositionOnTrajectoryPlane + "   " + newPositionOnLine + "   " + newHorizontalPosition + "   " + newPositionWithoutVerticalChange + "    " + targetPosition);
            //Debug.Log(manager.transform.position + "    " + target.position + "    " + H_completion + "     " + yPos);

            MyComponents.BallState.transform.position = Vector3.MoveTowards(newPositionWithoutVerticalChange, targetPosition, SPEED * Time.fixedDeltaTime);
        }

        private void EndPassIfBallCaught(bool newUncatchable)
        {
            Assert.IsFalse(newUncatchable);
            if (newUncatchable == false)
            {
                MyComponents.BallState.UncatchableChanged -= EndPassIfBallCaught;
                MyComponents.BallState.PassTarget = Players.INVALID_PLAYER_ID;
            }
        }
    }
}
