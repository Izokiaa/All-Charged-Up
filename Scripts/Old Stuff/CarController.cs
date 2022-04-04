using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CarController : MonoBehaviour
{
    public float acceleration;
    public float steering;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() //update = bad for physics
    {
        float x = -Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector2 speed = transform.up * (y * acceleration);
        rb.AddForce(speed);

        float direction = Vector2.Dot(rb.velocity, rb.GetRelativeVector(Vector2.up));
        if (direction >= 0.0f)
        {
            rb.rotation += x * steering * (rb.velocity.magnitude / 5.0f);
            //rb.rotation = ((1 + rb.velocity.magnitude) / 3) * Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));
            //rb.AddTorque((h * steering) * (rb.velocity.magnitude / 10.0f));
        }
        else
        {
            rb.rotation -= x * steering * (rb.velocity.magnitude / 5.0f);
            //rb.rotation = ((1 + rb.velocity.magnitude) / 3) * Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));
            //rb.AddTorque((-h * steering) * (rb.velocity.magnitude / 10.0f));
        }

        Vector2 forward = new Vector2(0.0f, 0.5f);
        float steeringRightAngle;
        if (rb.angularVelocity > 0)
        {
            steeringRightAngle = -90;
        }
        else
        {
            steeringRightAngle = 90;
        }


        Vector2 rightAngleFromCurDir = Quaternion.AngleAxis(steeringRightAngle, Vector3.forward) * forward;

        Debug.DrawLine((Vector3)rb.position, (Vector3)rb.GetRelativePoint(rightAngleFromCurDir), Color.green);

        float driftForce = Vector2.Dot(rb.velocity, rb.GetRelativeVector(rightAngleFromCurDir.normalized));
        //float driftForce = ((1 + rb.velocity.magnitude) / 3) * Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));
        Vector2 relativeForce = (rightAngleFromCurDir.normalized * -1.0f) * (driftForce * 10.0f);


        Debug.DrawLine((Vector3)rb.position, (Vector3)rb.GetRelativePoint(relativeForce), Color.red);

        rb.AddForce(rb.GetRelativeVector(relativeForce));
    }
}
