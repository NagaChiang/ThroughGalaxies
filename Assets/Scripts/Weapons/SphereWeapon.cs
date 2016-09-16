using UnityEngine;
using System.Collections;

public class SphereWeapon : PlayerWeapon {

    public float cooldownReducted;
    public GameObject sphereArea;
    public GameObject sphereAreaEnhanced;
    public GameObject sphereAreaUltimate;

    protected override IEnumerator doFire(float fireOffsetAngle = 0)
    {
        // fire depending on current level
        Quaternion rot = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y, 0.0f);
        switch (level)
        {
            case 1:
                Instantiate(bullet, transform.position, rot);
                // NOTE: didn't set back the CD
                break;

            case 2:
                // CDR
                fireCooldown = cooldownReducted;
                Instantiate(bullet, transform.position, rot);
                break;

            case 3:
                // CDR + area damage
                fireCooldown = cooldownReducted;
                Instantiate(sphereArea, transform.position, rot);
                break;

            case 4:
                // CDR + area damage + attack up
                fireCooldown = cooldownReducted;
                Instantiate(sphereAreaEnhanced, transform.position, rot);
                break;

            case 5:
                // CDR + area damage + attack up + remain
                fireCooldown = cooldownReducted;
                Instantiate(sphereAreaUltimate, transform.position, rot);
                break;

            default:
                Debug.LogWarning("Invalid weapon level: " + level.ToString());
                break;
        }

        yield break;
    }
}
