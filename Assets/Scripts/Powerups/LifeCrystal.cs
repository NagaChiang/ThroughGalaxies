using UnityEngine;
using System.Collections;

public class LifeCrystal : Powerup {

    public int life;

    public override void doPowerup(PlayerController player)
    {
        player.addLife(life);
    }
}
