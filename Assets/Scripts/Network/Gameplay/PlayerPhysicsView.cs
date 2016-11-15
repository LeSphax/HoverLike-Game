using UnityEngine;

public class PlayerPhysicsView : PhysicsView
{
    [HideInInspector]
    public bool Activated = false;

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

    protected void Start()
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
        UpdateView();
    }

    protected override void ServerBehaviour()
    {
        UpdateView();
    }

    private void UpdateView()
    {
        if (Activated)
        {
            transform.position = Vector3.MoveTowards(transform.position, model.transform.position, DashEffect.SPEED * 1.2f * Time.fixedDeltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, model.transform.rotation, model.AngularSpeed * 1.5f * Time.fixedDeltaTime);
            if (IsMyPlayer())
                CameraController.transform.position = transform.position;
        }
    }

    private bool IsMyPlayer()
    {
        return controller != null && controller.Player != null && controller.Player.IsMyPlayer;

    }
}
