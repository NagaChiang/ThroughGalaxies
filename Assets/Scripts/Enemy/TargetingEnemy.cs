using UnityEngine;
using System.Collections;

public class TargetingEnemy : Enemy {

    public float initialSpeed;
    public float verticalSpeed;
    public SimpleWeapon weapon;
    public float firingDelay;
    public Limit durationStart;

    private Rigidbody _rigidbody;

    new void Start()
    {
        // from Enemy (also Damageable)
        base.Start();

        // move down (-z)
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.velocity = Vector3.back * initialSpeed;

        // keep looking at the player and firing
        StartCoroutine(targeting());
    }

    private IEnumerator targeting()
    {
        // start wait
        yield return new WaitForSeconds(Random.Range(durationStart.min, durationStart.max));

        // change the velocity
        _rigidbody.velocity = Vector3.back * verticalSpeed;

        // keep firing
        StartCoroutine(keepFiring());
        
        // keep targeting player
        GameObject objPlayer = GameObject.FindWithTag("Player");
        if (objPlayer == null)
            Debug.LogError("Can't find the object of player.");
        else
        {
            while (objPlayer)
            {
                transform.LookAt(objPlayer.transform.position);
                yield return null;
            }
        }
    }

    private IEnumerator keepFiring()
    {
        // little delay for look at
        yield return new WaitForSeconds(firingDelay);

        while (true)
        {
            weapon.fire();
            yield return new WaitForSeconds(weapon.fireCooldown);
        }
    }
}
