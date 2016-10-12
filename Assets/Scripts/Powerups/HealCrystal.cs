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

        // show popup text
        PopupTextManager popupManager = GameObject.FindWithTag("PopupTextManager").GetComponent<PopupTextManager>();
        if (popupManager)
            popupManager.showMessage("+" + healing + " HP", player.transform.position);
        else
            Debug.LogError("Can't find the PopupTextManager.");
    }
}
