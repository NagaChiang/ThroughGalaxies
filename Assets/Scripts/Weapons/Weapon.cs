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

    public void aimFire(Vector3 posTarget)
    {
        // calculate the angle to target
        Quaternion quat = Quaternion.FromToRotation(transform.forward, posTarget - transform.position);
        float angle = quat.eulerAngles.y;

        // pass to fire
        fire(angle);
    }

    protected abstract IEnumerator doFire(float fireOffsetAngle = 0);
}
