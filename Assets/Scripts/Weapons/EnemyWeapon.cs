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
                Instantiate(bullet, transform.position, transform.rotation);
            }
            else
            {
                // spread bullet in one shot
                float angleBetweenBullet = shotAngleRange / (bulletPerShot - 1);
                Quaternion lookRotation = transform.rotation;
                lookRotation.SetLookRotation(transform.forward, Vector3.up); // get rid off tilting
                Quaternion quatStart = lookRotation * Quaternion.Euler(0.0f, -shotAngleRange / 2, 0.0f);
                for (int j = 0; j < bulletPerShot; j++)
                {
                    Quaternion rot = quatStart * Quaternion.Euler(0.0f, j * angleBetweenBullet, 0.0f);
                    Instantiate(bullet, transform.position, rot);
                }
            }

            // shot interval
            yield return new WaitForSeconds(shotInterval);
        }
    }
}
