using UnityEngine;
using System.Collections;

public class HealCrystal : Powerup
{
    public int healing;
    public int lootWeight;

    public override void doPowerup(PlayerController player)
    {
        // heal the player
        player.applyHealing(healing);
    }
}
