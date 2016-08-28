using UnityEngine;
using System.Collections;

[System.Serializable]
public struct VerticalSpeed
{
    public float min;
    public float max;
}

public class Asteroid : Damageable {

    public VerticalSpeed verticalSpeed;
    public float rotateFactor;
    public GameObject vfxExplosion;

	new void Start ()
    {
        // from Damageable
        base.Start();

        // random speed
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = new Vector3(0.0f, 0.0f, Random.Range(verticalSpeed.min, verticalSpeed.max));

        // random rotation
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
