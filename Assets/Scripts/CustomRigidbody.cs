using System;
using UnityEngine;

public class CustomRigidbody : MonoBehaviour
{
    private CharacterController _characterController;
    public CharacterController characterController
    {
        get
        {
            if (this._characterController == null)
            {
                this._characterController = this.GetComponent<CharacterController>();
            }

            return this._characterController;
        }
    }

    [NonSerialized]
    public Vector3 velocity;
    [NonSerialized]
    public Vector3 acceleration = Vector3.zero;

    [NonSerialized]
    public bool activated = true;

    public float bounciness;
    public float drag;

    public void Simulate(float dt)
    {
        if (this.characterController == null)
        {
            return;
        }
        //Debug.Log(velocity + "   " + acceleration);
        if (activated)
        {
            if (gameObject.name == "Ball(Clone)")
                Debug.Log("Before " + velocity);
            Vector3 movementDelta = (this.velocity * dt) + 0.5f * (Physics.gravity * dt * dt);


            this.velocity += Physics.gravity * dt;
            velocity += acceleration * dt;
            velocity *= (1 - drag * dt);
            acceleration = Vector3.zero;
            //Debug.Log(velocity);
            this.characterController.Move(movementDelta);
            if (gameObject.name == "Ball(Clone)")
                Debug.Log("After " + velocity);
        }
    }

    internal void AddForce(Vector3 vector3)
    {
        acceleration += vector3;
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        float collisionForceMagnitude = Mathf.Cos(Mathf.Deg2Rad * Vector3.Angle(velocity, hit.normal)) * velocity.magnitude;
        velocity += 2 * collisionForceMagnitude * bounciness * -hit.normal;
    }

    public virtual void FixedUpdate()
    {
        this.Simulate(Time.fixedDeltaTime);
    }
}