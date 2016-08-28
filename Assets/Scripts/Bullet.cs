using UnityEngine;
using System.Collections;

public enum BulletSource
{
    player,
    enemy,
}

public class Bullet : MonoBehaviour {

    public BulletSource bulletSource;
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
        // ignore boundary and other bullets 
        if (other.tag == "Boundary"
            || other.tag == "Bullet")
        {
            // do nothing
            return;
        }

        // ignore player (bullets from player)
        else if (other.tag == "Player"
                    && bulletSource == BulletSource.player)
        {
            // do nothing
            return;
        }

        // ignore enemies (bullets from enemies)
        else if (other.tag == "Enemy"
                    && bulletSource == BulletSource.enemy)
        {
            // do nothing
            return;
        }

        // hit anything else Damageable
        else
        {
            // apply damage to it
            Damageable target = other.GetComponent<Damageable>();
            if(target != null)
                target.applyDamage(damage);

            // destroy this bullet
            Destroy(gameObject);
        }
    }
}
