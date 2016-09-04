using UnityEngine;
using System.Collections;

public class SimpleWeapon : Weapon {

    protected override IEnumerator doFire()
    {
        Instantiate(bullet, transform.position, transform.rotation);
        yield break;
    }
}
