using UnityEngine;
using System.Collections;

// NOTE: Must instantiate a game object in the scene;
// otherwise, it can not trace the position of the attached object.

public abstract class Weapon : SfxBase
{
    [Header("Weapon")]
    public GameObject bullet;
    public float fireCooldown;
    public float FireDuration;

    [Header("More Sfx")]
    public AudioClip Clip_LockOn;
    public AudioClip Clip_Fire;

    public float NextFire { get; set; }

    public void fire(float fireOffsetAngle = 0)
    {
        // check cooldown and fire
        if(Time.time > NextFire)
        {
            NextFire = Time.time + fireCooldown;
            StartCoroutine(doFire(fireOffsetAngle));

            // Get audio manager again if needed
            if(Audio == null)
            {
                // Get audio manager
                GameManager gameMgr = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
                Audio = gameMgr.AudioManager;
            }

            // Sfx
            if (Clip_LockOn)
                Audio.PlaySfx(Clip_LockOn);
            if (Clip_Fire)
                Audio.PlaySfx(Clip_Fire);
        }
    }

    public void aimFire(GameObject obj)
    {
        if (obj)
        {
            // calculate the angle to target
            Vector3 pos = obj.transform.position;
            Quaternion quat = Quaternion.FromToRotation(transform.forward, pos - transform.position);
            float angle = quat.eulerAngles.y;

            // pass to fire
            fire(angle);
        }
    }

    public virtual void endFire() { }
    protected abstract IEnumerator doFire(float fireOffsetAngle = 0);
}
