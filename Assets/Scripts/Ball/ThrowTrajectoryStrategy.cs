using System.Linq;
using TimeSlow;
using UnityEngine;
using UnityEngine.Assertions;

namespace Ball
{
    public class ThrowTrajectoryStrategy : BallTrajectoryStrategy
    {
        public override Vector3 CurrentVelocity
        {
            get
            {
                Vector3 finalDirection;
                float curveProportion = speed * Time.fixedDeltaTime / curveLength;
                if (previousCompletion > speed * Time.fixedDeltaTime / curveLength)
                    finalDirection = (BezierMaths.Bezier3(controlPoints, previousCompletion) - BezierMaths.Bezier3(controlPoints, previousCompletion - curveProportion)).normalized;
                else
                    finalDirection = (BezierMaths.Bezier3(controlPoints, previousCompletion + curveProportion) - BezierMaths.Bezier3(controlPoints, previousCompletion)).normalized;
                return finalDirection * speed;
            }
        }

        Vector3[] controlPoints;
        float curveLength;
        float previousCompletion;
        public static float ShootPowerLevel = 250;
        float speed;

        public ThrowTrajectoryStrategy(Vector3[] controlPoints, float power)
        {
            Assert.IsTrue(MyComponents.NetworkManagement.IsServer);
            MyComponents.BallState.DetachBall();
            MyComponents.BallState.UnCatchable = false;
            AttractionBall.Activated = false;
            MyComponents.BallState.TrySetKinematic();
            MyComponents.BallState.transform.SetParent(null);

            this.curveLength = BezierMaths.LengthBezier3(controlPoints, 10);
            this.speed = BallMovementView.GetShootPowerLevel(power);
            this.controlPoints = controlPoints;
            previousCompletion = 0;
        }
        public override void MoveBall()
        {
            float increment = speed * Time.fixedDeltaTime / curveLength;
            float newCompletion = previousCompletion + increment;
            if (newCompletion <= 1)
            {
                Vector3 previousPosition = MyComponents.BallState.transform.position;
                Vector3 newPosition = BezierMaths.Bezier3(controlPoints, newCompletion);
                newPosition = new Vector3(
                    newPosition.x,
                    MyComponents.BallState.transform.position.y,
                    newPosition.z
                    );



                float displacement = Vector3.Distance(newPosition, previousPosition) / Time.fixedDeltaTime;
                float reducedProportion = (displacement * Time.fixedDeltaTime * MyComponents.BallState.Rigidbody.drag) / displacement;
                //The TimeSlowApplier effect doesn't matter since we depend on the previousCompletion
                //It still takes cares of having the ball fall twice as slow though
                if (TimeSlowApplier.ObjectsBeforeUpdate.Keys.Contains(MyComponents.BallState.Rigidbody))
                {
                    increment *= TimeSlowApplier.PlayerSlowProportion;
                    newCompletion = previousCompletion + increment;
                    newPosition = BezierMaths.Bezier3(controlPoints, newCompletion);
                    newPosition = new Vector3(
                        newPosition.x,
                        MyComponents.BallState.transform.position.y,
                        newPosition.z
                        );
                    reducedProportion *= TimeSlowApplier.PlayerSlowProportion;
                }
                speed *= 1 - reducedProportion;
                previousCompletion = newCompletion;

                if (CheckIfGoingThroughWall(previousPosition, newPosition))
                {
                    StopControlledTrajectory();
                }
                else
                    MyComponents.BallState.transform.position = newPosition;
            }
            else
            {
                StopControlledTrajectory();
            }
        }

        private bool CheckIfGoingThroughWall(Vector3 previousPosition, Vector3 newPosition)
        {
            Ray ray = new Ray(previousPosition, newPosition - previousPosition);
            RaycastHit hit;
            LayerMask layerMask = (1 << Layers.Default);
            //GizmosDrawer.DrawRay(ray);
            //GizmosDrawer.DrawLine(previousPosition, newPosition);
            //EditorApplication.isPaused = true;
            if (Physics.Raycast(ray, out hit, Vector3.Distance(newPosition, previousPosition), layerMask))
            {
                //Debug.Log(ray.origin);
                //Debug.Log(ray.direction);
                //Debug.Log(previousPosition);
                //Debug.Log(newPosition);
                //Debug.Log(hit.distance);
                //Debug.Log(hit.point);
                //Debug.Log(hit.collider.name);
                return true;
            }
            return false;
        }

        private void StopControlledTrajectory()
        {
            MyComponents.BallState.Rigidbody.velocity = new Vector3(CurrentVelocity.x, MyComponents.BallState.Rigidbody.velocity.y, CurrentVelocity.z);
            MyComponents.BallState.trajectoryStrategy = new FreeTrajectoryStrategy();
            //EditorApplication.isPaused = true;

        }
    }
}