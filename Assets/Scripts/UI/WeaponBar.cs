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
        changeColor(weapon.color);
    }

    // change values instantly
    public void switchWeapon(PlayerWeapon weapon)
    {
        // stop all lerping
        StopAllCoroutines();
        isLerping = false;

        // set values
        float proportion = (float)weapon.experience / weapon.getExpForNextLevel();
        if (weapon.isMaxLevel())
            proportion = 1.0f;
        if (proportion < 0)
            proportion = 0.0f;
        imageBar.fillAmount = proportion;
        imageDelayedBar.fillAmount = proportion;

        // update number of level
        textLevel.text = weapon.level.ToString();

        // update colors
        changeColor(weapon.color);
    }

    private void changeColor(Color color)
    {
        imageDelayedBar.color = new Color(color.r, color.g, color.b, imageDelayedBar.color.a);
        imageBar.color = color;
        imageWeaponIndicator.color = color;
    }
}
