using UnityEngine;
using System.Collections;

// NOTE: Must instantiate a game object in the scene;
// otherwise, it can not trace the position of the attached object.

public abstract class Weapon : MonoBehaviour {

    public GameObject bullet;
    public float fireCooldown;

    private float _nextFire;

    public void fire(float fireOffsetAngle = 0)
    {
        // check cooldown and fire
        if(Time.time > _nextFire)
        {
            _nextFire = Time.time + fireCooldown;
            StartCoroutine(doFire(fireOffsetAngle));
        }
    }

    protected abstract IEnumerator doFire(float fireOffsetAngle = 0);
}
