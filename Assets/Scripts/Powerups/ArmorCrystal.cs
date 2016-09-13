using UnityEngine;
using System.Collections;

public class ArmorCrystal : Powerup {

    public int armor;

    public override void doPowerup(PlayerController player)
    {
        // heal the player
        player.addArmor(armor);
    }
}
