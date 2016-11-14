using PlayerManagement;
using System;
using UnityEngine;

public class BallPhysicsView : PhysicsView
{
    public BallPhysicsModel model;

    protected override void ClientBehaviour()
    {
        UpdateView();
    }

    private void UpdateView()
    {
        if (!MyComponents.BallState.IsAttached())
            transform.position = Vector3.MoveTowards(transform.position, model.transform.position, model.MAX_SPEED * 1.5f * Time.fixedDeltaTime);
    }

    protected override void ServerBehaviour()
    {
        UpdateView();
    }
}