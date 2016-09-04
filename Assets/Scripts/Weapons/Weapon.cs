using UnityEngine;
using System.Collections;

// NOTE: Must instantiate a game object in the scene;
// otherwise, it can not trace the position of the attached object.

public abstract class Weapon : MonoBehaviour {

    public GameObject bullet;
    public float fireCooldown;
    public int shotPerFire;
    public float shotInterval;

    private float _nextFire;

    public void fire()
    {
        // check cooldown and fire
        if(Time.time > _nextFire)
        {
            _nextFire = Time.time + fireCooldown;
            StartCoroutine(doFire());
        }
    }

    protected abstract IEnumerator doFire();
}
