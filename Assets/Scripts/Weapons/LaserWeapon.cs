using UnityEngine;
using System.Collections;

public class LaserWeapon : PlayerWeapon {

    private GameObject Laser;

    // When player release the key, end the laser
    public void endFire()
    {
        Destroy(Laser);
    }

    protected override IEnumerator doFire(float fireOffsetAngle = 0)
    {
        // fire depending on current level
        switch (level)
        {
            case 1:
                if (!Laser)
                {
                    Laser = (GameObject)Instantiate(bullet, transform.position, transform.rotation);
                    Laser.transform.SetParent(transform);
                }
                break;

            case 2:
                if (!Laser)
                {
                    Laser = (GameObject)Instantiate(bullet, transform.position, transform.rotation);
                    Laser.transform.SetParent(transform);
                }
                break;

            case 3:
                if (!Laser)
                {
                    Laser = (GameObject)Instantiate(bullet, transform.position, transform.rotation);
                    Laser.transform.SetParent(transform);
                }
                break;

            case 4:
                if (!Laser)
                {
                    Laser = (GameObject)Instantiate(bullet, transform.position, transform.rotation);
                    Laser.transform.SetParent(transform);
                }
                break;

            case 5:
                if (!Laser)
                {
                    Laser = (GameObject)Instantiate(bullet, transform.position, transform.rotation);
                    Laser.transform.SetParent(transform);
                }
                break;

            default:
                Debug.LogWarning("Invalid weapon level: " + level.ToString());
                break;
        }

        yield break;
    }
}
