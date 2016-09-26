using UnityEngine;
using System.Collections;

public class TargetingEnemy : Enemy {

    [Header("Targeting")]
    public float initialSpeed;
    public float verticalSpeed;
    public float firingDelay;
    public Limit durationStart;
    public bool enabledTargetingOnce;

    private Rigidbody _rigidbody;

    protected override void Start()
    {
        // from Enemy (also Damageable)
        base.Start();

        // move down (-z)
        if (initialSpeed > 0)
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.velocity = Vector3.back * initialSpeed;
        }

        // keep looking at the player and firing
        StartCoroutine(targeting());
    }

    private IEnumerator targeting()
    {
        // start wait
        yield return new WaitForSeconds(Random.Range(durationStart.min, durationStart.max));

        // change the velocity
        GetComponent<Rigidbody>().velocity = Vector3.back * verticalSpeed;

        // keep firing
        foreach (Weapon weapon in weapons)
            StartCoroutine(keepFiring(weapon));

        // keep targeting player
        GameObject objPlayer = GameObject.FindWithTag("Player");
        if (objPlayer == null)
            Debug.Log("Can't find the object of player.");
        else
        {
            while (objPlayer)
            {
                // if player not respawning (temporary moved to higher place)
                if (objPlayer.transform.position.y == 0)
                {
                    transform.LookAt(objPlayer.transform.position, transform.up);

                    // Targeting once
                    if (enabledTargetingOnce)
                        yield break;
                }

                yield return null;
            }
        }
    }

    private IEnumerator keepFiring(Weapon weapon)
    {
        // little delay for look at
        yield return new WaitForSeconds(firingDelay);

        while (true)
        {
            // Fire
            weapon.fire();
            yield return new WaitForSeconds(weapon.fireCooldown);
        }
    }
}
