using UnityEngine;
using System.Collections;

public class LaserWeapon : PlayerWeapon {

    [Header("LaserWeapon")]
    public GameObject BulletLv2;
    public GameObject BulletLv3;
    public GameObject BulletLv4;
    public GameObject BulletLv5;
    public bool EnabledShaking;

    private GameObject LaserInstance;

    // When player release the key, end the laser
    public override void endFire()
    {
        if(LaserInstance)
            Destroy(LaserInstance);
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
        if (!LaserInstance)
        {
            switch (level)
            {
                case 1:
                    LaserInstance = (GameObject)Instantiate(bullet, transform.position, transform.rotation);
                    LaserInstance.transform.SetParent(transform);
                    break;

                case 2:
                    LaserInstance = (GameObject)Instantiate(BulletLv2, transform.position, transform.rotation);
                    LaserInstance.transform.SetParent(transform);
                    break;

                case 3:
                    LaserInstance = (GameObject)Instantiate(BulletLv3, transform.position, transform.rotation);
                    LaserInstance.transform.SetParent(transform);
                    break;

                case 4:
                    LaserInstance = (GameObject)Instantiate(BulletLv4, transform.position, transform.rotation);
                    LaserInstance.transform.SetParent(transform);
                    break;

                case 5:
                    LaserInstance = (GameObject)Instantiate(BulletLv5, transform.position, transform.rotation);
                    LaserInstance.transform.SetParent(transform);
                    break;

                default:
                    Debug.LogWarning("Invalid weapon level: " + level.ToString());
                    break;
            }
        }

        // Shaking!
        Laser laser = LaserInstance.GetComponent<Laser>();
        if (EnabledShaking)
        {
            CameraShaker camera = GameObject.FindWithTag("GameManager").GetComponent<GameManager>().Camera;
            yield return new WaitForSeconds(laser.StartDelay);
            camera.SetShake(0.1f, FireDuration - laser.StartDelay, 0.5f);
        }

        // End the laser
        if (FireDuration > 0)
        {
            if (EnabledShaking)
                yield return new WaitForSeconds(FireDuration - laser.StartDelay);
            else
                yield return new WaitForSeconds(FireDuration);

            endFire();
        }
    }
}
