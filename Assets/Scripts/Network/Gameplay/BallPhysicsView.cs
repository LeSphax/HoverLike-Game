using PlayerManagement;
using System;
using UnityEngine;

public class BallPhysicsView : PhysicsView
{
    public BallPhysicsModel model;

    protected override void ClientBehaviour()
    {
        //if (model.PlayerOwningBall != Players.INVALID_PLAYER_ID)
        //{
        //    float distance = Vector3.Distance(transform.position, model.transform.position);
        //    transform.position = Vector3.MoveTowards(transform.position, model.transform.position, Mathf.distance / 2);
        //}
        //else
            transform.position = model.transform.position;
    }

    protected override void ServerBehaviour()
    {
        transform.position = model.transform.position;
    }
}