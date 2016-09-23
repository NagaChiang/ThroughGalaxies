using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

    [Header("Laser")]
    public BulletSource BulletSource;
    public float Width;
    public float DamagePerSecond;
    public float DamageInterval;

    [Header("VFX")]
    public LineRenderer LineLaser;
    public ParticleSystem LaserBurn; 

    private float NextDamageTime;

    void Update()
    {
        // Follow parent
        transform.localPosition = Vector3.zero;

        // Set width and first point
        LineLaser.SetWidth(Width, Width);
        LineLaser.SetPosition(0, transform.position);

        // Raycast to set length
        float laserLength = 0.0f;
        float maxLength = 50.0f;
        RaycastHit hitInfo;
        Vector3 boxCenter = Vector3.zero;
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, maxLength))
        {
            // Update line renderer
            LineLaser.SetPosition(1, hitInfo.point);

            // Box for damage
            boxCenter = (transform.position + hitInfo.point) / 2.0f;
            laserLength = hitInfo.distance;

            // Play particle system to show burning
            LaserBurn.transform.position = hitInfo.point + 5.0f * Vector3.up;
            if(LaserBurn.isStopped)
                LaserBurn.Play();
        }
        else
        {
            // Not hitting anything, use max length
            LineLaser.SetPosition(1, transform.position + maxLength * transform.forward);

            // Box for damage
            boxCenter = transform.position + (maxLength / 2.0f) * transform.forward;
            laserLength = maxLength;

            // Stop particle system of burning
            if (LaserBurn.isPlaying)
                LaserBurn.Stop();
        }


        // Damage
        if (Time.time >= NextDamageTime)
        {
            Collider[] colliders = Physics.OverlapBox(boxCenter, new Vector3(Width / 2.0f, 0.0f, laserLength / 2.0f));
            foreach (Collider collider in colliders)
            {
                // check null
                if (collider == null)
                    continue;

                // ignore boundary and other bullets 
                if (collider.tag == "Boundary"
                    || collider.tag == "Bullet"
                    || collider.tag == "Powerup")
                {
                    // do nothing
                    continue;
                }

                // ignore player (bullets from player)
                else if (collider.tag == "Player"
                            && BulletSource == BulletSource.player)
                {
                    // do nothing
                    continue;
                }

                // ignore enemies (bullets from enemies)
                else if (collider.tag == "Enemy"
                            && BulletSource == BulletSource.enemy)
                {
                    // do nothing
                    continue;
                }

                // hit anything else Damageable
                else
                {
                    // apply damage to it
                    Damageable target = collider.GetComponent<Damageable>();
                    if (target != null)
                        target.applyDamage((int)(DamagePerSecond * DamageInterval));
                }
            }

            // Update next damage time
            NextDamageTime = Time.time + DamageInterval;
        }
    }
}
