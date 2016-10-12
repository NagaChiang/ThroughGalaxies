using UnityEngine;
using System.Collections;

public class ArmorCrystal : Powerup {

    public int armor;

    public override void doPowerup(PlayerController player)
    {
        // heal the player
        player.addArmor(armor);

        // show popup text
        PopupTextManager popupManager = GameObject.FindWithTag("PopupTextManager").GetComponent<PopupTextManager>();
        if (popupManager)
            popupManager.showMessage("+" + armor + " MAX HP", player.transform.position);
        else
            Debug.LogError("Can't find the PopupTextManager.");
    }
}
