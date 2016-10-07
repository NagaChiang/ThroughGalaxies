using UnityEngine;
using System.Collections;

// fire > shot > bullet

public class EnemyWeapon : Weapon {

    [Header("Advanced")]
    public float shotInterval;
    public int shotPerFire;
    public float bulletInterval;
    public int bulletPerShot;
    public float shotAngleRange;

    protected override IEnumerator doFire(float fireOffsetAngle = 0)
    {
        for (int i = 0; i < shotPerFire; i++)
        {
            // SFX for more than one shot
            if(Clip_Fire && i > 0)
                Audio.PlaySfx(Clip_Fire);

            if (bulletPerShot <= 1 || shotAngleRange == 0)
            {
                // straight shot
                Vector3 pos = new Vector3(transform.position.x, 0.0f, transform.position.z);
                Quaternion rot = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y + fireOffsetAngle, 0.0f);
                Instantiate(bullet, pos, rot);
            }
            else
            {
                // spread bullet in one shot
                if (shotAngleRange > 360.0f)
                    shotAngleRange = 360.0f;

                float angleBetweenBullet = 0.0f;
                if (shotAngleRange == 360.0f) // whole circle, start and end are the same
                    angleBetweenBullet = shotAngleRange / bulletPerShot;
                else
                    angleBetweenBullet = shotAngleRange / (bulletPerShot - 1);

                Quaternion lookRotation = transform.rotation;
                lookRotation.SetLookRotation(transform.forward, Vector3.up); // get rid off tilting
                Quaternion quatStart = lookRotation * Quaternion.Euler(0.0f, fireOffsetAngle - shotAngleRange / 2, 0.0f);
                for (int j = 0; j < bulletPerShot; j++)
                {
                    Vector3 pos = new Vector3(transform.position.x, 0.0f, transform.position.z);
                    Quaternion rot = quatStart * Quaternion.Euler(0.0f, j * angleBetweenBullet, 0.0f);
                    Instantiate(bullet, pos, rot);

                    // bullet interval
                    if(bulletInterval > 0)
                        yield return new WaitForSeconds(bulletInterval);
                }
            }

            // shot interval
            if(shotInterval > 0)
                yield return new WaitForSeconds(shotInterval);
        }
    }
}
