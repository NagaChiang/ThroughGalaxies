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
        // hit player or enemy
        if(other.tag == "Player" || other.tag == "Enemy")
        {
            // apply damage depending on remaining health
            Damageable target = other.GetComponent<Damageable>();
            if (target != null)
                target.applyDamage(_health);

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
