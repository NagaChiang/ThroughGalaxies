using UnityEngine;
using System.Collections;

public class ExpCrystal : Powerup {

    public int experience;

    public override void doPowerup(PlayerController player)
    {
        // add experience to player
        player.addWeaponExp(experience);
    }
}
