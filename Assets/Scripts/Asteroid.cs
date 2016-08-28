using UnityEngine;
using System.Collections;

public class Asteroid : Damageable {

    public float rotateFactor;
    public GameObject vfxExplosion;

	new void Start ()
    {
        // from Damageable
        base.Start();

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

    protected override void destroy()
    {
        // explosion vfx
        Instantiate(vfxExplosion, transform.position, transform.rotation);

        // destroy this asteroid
        Destroy(gameObject);
    }
}
