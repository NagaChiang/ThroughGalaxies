using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

    public float fireCooldown;
    public GameObject bullet;

    private float nextFire;

    public void fire()
    {
        // check cooldown and fire
        if(Time.time > nextFire)
        {
            nextFire = Time.time + fireCooldown;
            Instantiate(bullet, transform.position, transform.rotation);
        }
    }

}
