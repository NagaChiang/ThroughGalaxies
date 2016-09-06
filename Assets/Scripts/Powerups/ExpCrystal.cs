using UnityEngine;
using System.Collections;
using System;

public class ExpCrystal : Powerup {

    public int experience;

    public override void doPowerup(PlayerController player)
    {
        // add experience to player
        player.addWeaponExp(experience);
    }
}
