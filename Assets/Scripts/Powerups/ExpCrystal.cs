using UnityEngine;
using System.Collections;
using System;

public class ExpCrystal : Powerup {

    public int experience;

    public override void doPowerup(PlayerController player)
    {
        // add experience to player
        player.addWeaponExp(experience);

        // show popup text
        PopupTextManager popupManager = GameObject.FindWithTag("PopupTextManager").GetComponent<PopupTextManager>();
        if (popupManager)
            popupManager.showMessage("+" + experience + " EXP", player.transform.position);
        else
            Debug.LogError("Can't find the PopupTextManager.");
    }
}
