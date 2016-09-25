using UnityEngine;
using System.Collections;

// NOTE: Must instantiate a game object in the scene;
// otherwise, it can not trace the position of the attached object.

public abstract class Weapon : MonoBehaviour {

    public GameObject bullet;
    public float fireCooldown;
    public float FireDuration;

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

    public void aimFire(GameObject obj)
    {
        // calculate the angle to target
        Vector3 pos = obj.transform.position;
        Quaternion quat = Quaternion.FromToRotation(transform.forward, pos - transform.position);
        float angle = quat.eulerAngles.y;

        // pass to fire
        fire(angle);
    }

    public virtual void endFire() { }
    protected abstract IEnumerator doFire(float fireOffsetAngle = 0);
}
