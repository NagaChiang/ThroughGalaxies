using UnityEngine;
using System.Collections;

public class SimpleWeapon : Weapon {

    protected override void doFire()
    {
        Instantiate(bullet, transform.position, transform.rotation);
    }
}
