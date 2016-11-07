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

    public Vector3 velocity;
    public Vector3 acceleration = Vector3.zero;

    public void Simulate(float dt)
    {
        if (this.characterController == null)
        {
            return;
        }
        Debug.Log(velocity + "   " + acceleration);
        Vector3 movementDelta = (this.velocity * dt) + 0.5f * (Physics.gravity * dt * dt);
        this.characterController.Move(movementDelta);

        this.velocity += Physics.gravity * dt;
        velocity += acceleration * dt;
        acceleration = Vector3.zero;
        Debug.Log(velocity);
    }

    internal void AddForce(Vector3 vector3)
    {
        acceleration += vector3;
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Negate some of the velocity based on the normal of the collision by projecting velocity onto normal
        Debug.Log("Collision " + velocity);
        //this.velocity += Vector3.Dot(this.velocity, hit.normal) * hit.normal;
        Debug.DrawRay(Vector3.up * 10, hit.normal, Color.red);
        Debug.Log("Collision " + velocity);
    }

    public virtual void FixedUpdate()
    {
        this.Simulate(Time.fixedDeltaTime);
    }
}