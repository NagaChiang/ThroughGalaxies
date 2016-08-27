using UnityEngine;
using System.Collections;

public class Asteroid : Damageable {

    public float rotateFactor;
    public GameObject vfxExplosion;

	void Start ()
    {
        // initial properties
        _health = maxHealth;

        // random rotation
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.angularVelocity = Random.insideUnitSphere * rotateFactor;
    }

    void OnTriggerEnter(Collider other)
    {
        // contact with boundary
        if (other.tag == "Boundary")
        {
            // do nothing
            return;
        }

        // contact with player
        if(other.tag == "Player")
        {
            // apply damage depending on remaining health
            other.GetComponent<Damageable>().applyDamage(_health);

            // destroy this asteroid
            destroy();
        }
    }

    // for other gameObject to apply damage
    public override void applyDamage(float damage)
    {
        // reduce health
        _health -= damage;

        // dead
        if (_health <= 0)
        {
            // destroy this asteroid
            destroy();
        }
    }

    protected override void destroy()
    {
        // explosion vfx
        Instantiate(vfxExplosion, transform.position, transform.rotation);

        // destroy this asteroid
        Destroy(gameObject);
    }
}
