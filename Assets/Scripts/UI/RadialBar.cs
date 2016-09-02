using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class RadialBar : MonoBehaviour {

    public Image imageBar;

    public void update(float value, float max)
    {
        // filled area of image bar
        updateFilledBar(value, max);
    }

    private void updateFilledBar(float value, float max)
    {
        // image bar filled area
        float proportion = value / max;
        if (proportion < 0)
            proportion = 0.0f;
        imageBar.fillAmount = proportion;
    }
}
