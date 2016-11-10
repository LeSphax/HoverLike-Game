using UnityEngine;

public class PlayerPhysicsView : PhysicsView
{
    public PlayerPhysicsModel model;
    public PlayerController controller;

    private GameObject cameraControlller;
    private GameObject CameraController
    {
        get
        {
            if (cameraControlller == null)
            {
                cameraControlller = GameObject.FindGameObjectWithTag(Tags.GameController);
            }
            return cameraControlller;
        }
    }

    protected override void ClientBehaviour()
    {
        //transform.position = Vector3.MoveTowards(transform.position, model.transform.position, 0.5f);
        UpdateView();
    }

    protected override void ServerBehaviour()
    {
        UpdateView();
    }

    private void UpdateView()
    {
        transform.position = model.transform.position;
        transform.rotation = model.transform.rotation;
        if (controller.Player.IsMyPlayer)
            CameraController.transform.position = transform.position;
    }
}
