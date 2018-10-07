using Byn.Net;
using PlayerManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Ball
{
    public class PassTrajectoryStrategy : BallTrajectoryStrategy
    {
        Vector3 passerPosition;
        Transform target;
        const float SPEED = 150;

        public PassTrajectoryStrategy(BallState ballState, Dictionary<ConnectionId, Player> players, ConnectionId passerId, ConnectionId targetId) : base(ballState)
        {
            BallState.DetachBall();
            BallState.UnCatchable = true;
            BallState.PassTarget = targetId;
            BallState.SetWorldAsParent();

            passerPosition = players[passerId].controller.transform.position;
            target = players[targetId].controller.transform;

            BallState.UncatchableChanged += EndPassIfBallCaught;
        }

        public override void MoveBall()
        {
            //On ramène la position de la balle sur la droite entre les deux joueurs qui se font la passe.
            Vector3 trajectoryPlaneNormal = Vector3.Normalize(Vector3.Cross(target.position - passerPosition, Vector3.up));
            Vector3 proj_ballPositionOnTrajectoryPlane = BallState.transform.position - Vector3.Dot(BallState.transform.position - target.position, trajectoryPlaneNormal) * trajectoryPlaneNormal;
            Vector3 proj_ballPositionOnTrajectoryLine = new Vector3(proj_ballPositionOnTrajectoryPlane.x, passerPosition.y, proj_ballPositionOnTrajectoryPlane.z);

            Vector3 newPositionOnLine = Vector3.MoveTowards(proj_ballPositionOnTrajectoryLine, target.position, SPEED * Time.fixedDeltaTime);

            //Proportion du mouvement horizontal effectué
            float HorizontalProportion = Vector3.Distance(passerPosition, newPositionOnLine) / Vector3.Distance(passerPosition, target.position);
            // A partir de la proportion du mouvement horizontal, on calcule la position verticale
            float yPos = target.position.y * HorizontalProportion + (1 - HorizontalProportion) * passerPosition.y;

            Vector3 newHorizontalPosition = new Vector3(newPositionOnLine.x, 0, newPositionOnLine.z);
            Vector3 targetPosition = newHorizontalPosition + Vector3.up * yPos;
            //Debug.Log(trajectoryPlaneNormal + "   " + proj_ballPositionOnTrajectoryLine + "   " + proj_ballPositionOnTrajectoryPlane + "   " + newPositionOnLine + "   " + newHorizontalPosition + "    " + targetPosition);
            //Debug.Log(manager.transform.position + "    " + target.position + "    " + H_completion + "     " + yPos);

            BallState.transform.position = Vector3.MoveTowards(BallState.transform.position, targetPosition, SPEED * Time.fixedDeltaTime);
        }

        private void EndPassIfBallCaught(bool newUncatchable)
        {
            Assert.IsFalse(newUncatchable);
            if (newUncatchable == false)
            {
                BallState.UncatchableChanged -= EndPassIfBallCaught;
                BallState.PassTarget = Players.INVALID_PLAYER_ID;
            }
        }
    }
}
