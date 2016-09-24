using UnityEngine;
using System.Collections;

public class LaserWeapon : PlayerWeapon {

    public GameObject BulletLv2;
    public GameObject BulletLv3;
    public GameObject BulletLv4;
    public GameObject BulletLv5;

    private GameObject Laser;

    // When player release the key, end the laser
    public void endFire()
    {
        Destroy(Laser);
    }

    // Using new bullet after upgrading
    public override bool addExperience(int exp)
    {
        bool isUpgraded = base.addExperience(exp);
        if (isUpgraded)
            endFire();

        return isUpgraded;
    }

    protected override IEnumerator doFire(float fireOffsetAngle = 0)
    {
        // fire depending on current level
        if (!Laser)
        {
            switch (level)
            {
                case 1:
                    Laser = (GameObject)Instantiate(bullet, transform.position, transform.rotation);
                    Laser.transform.SetParent(transform);
                    break;

                case 2:
                    Laser = (GameObject)Instantiate(BulletLv2, transform.position, transform.rotation);
                    Laser.transform.SetParent(transform);
                    break;

                case 3:
                    Laser = (GameObject)Instantiate(BulletLv3, transform.position, transform.rotation);
                    Laser.transform.SetParent(transform);
                    break;

                case 4:
                    Laser = (GameObject)Instantiate(BulletLv4, transform.position, transform.rotation);
                    Laser.transform.SetParent(transform);
                    break;

                case 5:
                    Laser = (GameObject)Instantiate(BulletLv5, transform.position, transform.rotation);
                    Laser.transform.SetParent(transform);
                    break;

                default:
                    Debug.LogWarning("Invalid weapon level: " + level.ToString());
                    break;
            }
        }

        yield break;
    }
}
