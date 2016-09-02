using UnityEngine;
using System.Collections;

public class PlayerWeapon : Weapon {

    public int experience { get; private set; }
    public int level { get; private set; }

    protected override void doFire()
    {
        Instantiate(bullet, transform.position, transform.rotation);
    }
}
