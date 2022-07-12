using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 velocity;

    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        transform.Translate(velocity * Time.deltaTime);
    }
    public void SetVelocity(Vector2 moveInput)
    {
        velocity = new Vector3(moveInput.x, 0, moveInput.y);
    }
}
