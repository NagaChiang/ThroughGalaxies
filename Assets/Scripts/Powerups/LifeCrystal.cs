using UnityEngine;
using System.Collections;

public class LifeCrystal : Powerup {

    public int life;

    public override void doPowerup(PlayerController player)
    {
        player.addLife(life);

        // show popup text
        PopupTextManager popupManager = GameObject.FindWithTag("PopupTextManager").GetComponent<PopupTextManager>();
        if (popupManager)
            popupManager.showMessage("+" + life + " LIFE", player.transform.position);
        else
            Debug.LogError("Can't find the PopupTextManager.");
    }
}
