using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WeaponBar : Bar {

    public Image imageWeaponIndicator;
    public Text textLevel;

    public void update(PlayerWeapon weapon)
    {
        // update filled bar
        if (weapon.isMaxLevel())
            base.update(1.0f, 1.0f);
        else
            base.update(weapon.experience, weapon.getExpForNextLevel());

        // update number of level
        textLevel.text = weapon.level.ToString();

        // update colors
        Color color = weapon.color;
        imageDelayedBar.color = new Color(color.r, color.g, color.b, imageDelayedBar.color.a);
        imageBar.color = color;
        imageWeaponIndicator.color = color;
    }
}
