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

    void OnTriggerEnter(Collider other)
    {
        // contact with boundary / player / bullets
        if (other.tag == "Boundary"
            || other.tag == "Player"
            || other.tag == "Bullet")
        {
            // do nothing
            return;
        }

        // contact with anything else (Damageable)
        else
        {
            // apply damage to it
            other.GetComponent<Damageable>().applyDamage(damage);

            // destroy this bullet
            Destroy(gameObject);
        }
    }
}
