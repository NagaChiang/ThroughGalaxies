using UnityEngine;
using System.Collections;

public class SpreadWeapon : Weapon {

    protected override IEnumerator doFire()
    {
        for (int i = 0; i < shotPerFire; i++)
        {
            Instantiate(bullet, transform.position, transform.rotation);
            yield return new WaitForSeconds(shotInterval);
        }
    }
}
