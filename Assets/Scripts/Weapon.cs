using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

    public float fireCooldown;
    public GameObject bullet;

    private float _nextFire;

    // copy constructor
    public Weapon(Weapon other)
    {
        fireCooldown = other.fireCooldown;
        bullet = other.bullet;
    }

    public void fire()
    {
        // check cooldown and fire
        if(Time.time > _nextFire)
        {
            _nextFire = Time.time + fireCooldown;
            Instantiate(bullet, transform.position, transform.rotation); // world position
        }
    }

}
