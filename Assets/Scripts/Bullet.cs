using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public float initialSpeed;
    public float acceleration;
    public float damage;

    void Start ()
    {
        // initial speed
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = transform.forward * initialSpeed;
    }

    void FixedUpdate()
    {
        // acceleration
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity += transform.forward * acceleration;
    }
}
