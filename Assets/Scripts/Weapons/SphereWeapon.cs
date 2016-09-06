using UnityEngine;
using System.Collections;

public class SphereWeapon : PlayerWeapon {

    public float cooldownReducted;
    public GameObject sphereArea;
    public GameObject sphereAreaEnhanced;
    public GameObject sphereAreaUltimate;

    protected override IEnumerator doFire()
    {
        // fire depending on current level
        switch (level)
        {
            case 1:
                Instantiate(bullet, transform.position, transform.rotation);
                break;

            case 2:
                // cooldown reduction
                fireCooldown = cooldownReducted;
                Instantiate(bullet, transform.position, transform.rotation);
                break;

            case 3:
                // CDR + area damage
                fireCooldown = cooldownReducted;
                Instantiate(sphereArea, transform.position, transform.rotation);
                break;

            case 4:
                // CDR + area damage + attack up
                fireCooldown = cooldownReducted;
                Instantiate(sphereAreaEnhanced, transform.position, transform.rotation);
                break;

            case 5:
                // CDR + area damage + attack up + remain
                fireCooldown = cooldownReducted;
                Instantiate(sphereAreaUltimate, transform.position, transform.rotation);
                break;

            default:
                Debug.LogWarning("Invalid weapon level: " + level.ToString());
                break;
        }

        yield break;
    }
}
