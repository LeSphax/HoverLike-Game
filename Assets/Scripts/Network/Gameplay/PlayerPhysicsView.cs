using UnityEngine;

public class PlayerPhysicsView : PhysicsView
{
    public PlayerPhysicsModel model;
    public PlayerController controller;

    public GameObject targetPrefab;
    private GameObject target;

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

    protected void Awake()
    {
        if (IsMyPlayer())
        {
            model.TargetPositionChanged += UpdateTarget;
            target = Instantiate(targetPrefab);
            target.SetActive(false);
        }
    }

    private void UpdateTarget(Vector3? position)
    {
        if (position == null)
        {
            target.SetActive(false);
        }
        else
        {
            target.SetActive(true);
            target.transform.position = position.Value;
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
        if (IsMyPlayer())
            CameraController.transform.position = transform.position;
    }

    private bool IsMyPlayer()
    {
        return controller != null && controller.Player != null && controller.Player.IsMyPlayer;

    }
}
