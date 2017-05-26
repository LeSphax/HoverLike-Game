using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    finalDirection = (Functions.Bezier3(controlPoints, previousCompletion) - Functions.Bezier3(controlPoints, previousCompletion - curveProportion)).normalized;
                else
                    finalDirection = (Functions.Bezier3(controlPoints, previousCompletion + curveProportion) - Functions.Bezier3(controlPoints, previousCompletion)).normalized;
                return finalDirection * speed;
            }
        }

        Vector3[] controlPoints;
        Vector3 previousPosition;
        float curveLength;
        float startingTime;
        float previousCompletion;
        public static float ShootPowerLevel = 250;
        float speed;

        public ThrowTrajectoryStrategy(Vector3[] controlPoints, float power)
        {
            Debug.Log("ThrowTrajectoryStrategy");
            Assert.IsTrue(MyComponents.NetworkManagement.isServer);
            MyComponents.BallState.DetachBall();
            MyComponents.BallState.UnCatchable = false;
            AttractionBall.Activated = false;
            MyComponents.BallState.TrySetKinematic();
            //MyComponents.BallState.protectionSphere.SetActive(false);
            MyComponents.BallState.transform.SetParent(null);

            this.curveLength = Functions.LengthBezier3(controlPoints, 10);
            this.speed = ShootPowerLevel * power;
            this.controlPoints = controlPoints;
            previousCompletion = 0;
            startingTime = Time.realtimeSinceStartup;
        }
        public override void MoveBall()
        {
            Debug.Log(speed + "    " + curveLength);
            float increment = speed * Time.fixedDeltaTime / curveLength;
            if (previousCompletion + increment <= 1)
            {
                //Vector3 originalCurvePosition = Functions.Bezier3(controlPoints, newCompletion);
                //Vector3 previousCurvePosition = Functions.Bezier3(controlPoints, previousCompletion);

                //Vector3 previousWorldPosition = new Vector3(
                //   previousCurvePosition.x,
                //   MyComponents.BallState.transform.position.y,
                //   previousCurvePosition.z
                //   );

                //Vector3 originalWorldPosition = new Vector3(
                //    originalCurvePosition.x,
                //    MyComponents.BallState.transform.position.y,
                //    originalCurvePosition.z
                //    );

                //float increment = (newCompletion - previousCompletion) * (speed * Time.fixedDeltaTime) / Vector3.Distance(previousWorldPosition, originalWorldPosition);


                //The TimeSlowApplier effect doesn't matter since we depend on the previousCompletion
                //It still takes cares of having the ball fall twice as slow though
                if (TimeSlowApplier.ObjectsBeforeUpdate.Keys.Contains(MyComponents.BallState.Rigidbody))
                {
                    increment *= TimeSlowApplier.TimeSlowProportion;
                }
                previousCompletion += increment;

                Vector3 previousPosition = MyComponents.BallState.transform.position;
                Vector3 newPosition = Functions.Bezier3(controlPoints, previousCompletion);
                MyComponents.BallState.transform.position = new Vector3(
                    newPosition.x,
                    MyComponents.BallState.transform.position.y,
                    newPosition.z
                    );
                float displacement = Vector3.Distance(MyComponents.BallState.transform.position, previousPosition) / Time.fixedDeltaTime;
                float reducedProportion = (displacement * Time.fixedDeltaTime * MyComponents.BallState.Rigidbody.drag) / displacement;
                Debug.Log(displacement + "     "  + reducedProportion);
                speed *= 1 - reducedProportion;
            }
            else
            {
                MyComponents.BallState.Rigidbody.velocity = new Vector3(CurrentVelocity.x, MyComponents.BallState.Rigidbody.velocity.y, CurrentVelocity.z);
                MyComponents.BallState.trajectoryStrategy = new FreeTrajectoryStrategy();
            }
            previousPosition = MyComponents.BallState.transform.position;
        }
    }
}