using UnityEngine;
using System.Collections;

// fire > shot > bullet

public class EnemyWeapon : Weapon {

    protected override IEnumerator doFire()
    {
        for (int i = 0; i < shotPerFire; i++)
        {
            if (bulletPerShot <= 1 || shotAngleRange == 0)
            {
                // straight shot
                Vector3 pos = new Vector3(transform.position.x, 0.0f, transform.position.z);
                Quaternion rot = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y, 0.0f);
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
                Quaternion quatStart = lookRotation * Quaternion.Euler(0.0f, -shotAngleRange / 2, 0.0f);
                for (int j = 0; j < bulletPerShot; j++)
                {
                    Vector3 pos = new Vector3(transform.position.x, 0.0f, transform.position.z);
                    Quaternion rot = quatStart * Quaternion.Euler(0.0f, j * angleBetweenBullet, 0.0f);
                    Instantiate(bullet, pos, rot);
                }
            }

            // shot interval
            yield return new WaitForSeconds(shotInterval);
        }
    }
}
