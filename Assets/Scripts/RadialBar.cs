using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RadialBar : MonoBehaviour {

    public Image imageBar;
    public Text textValue;
    public Text textValueMax;

    public void updateValues(float value, float max)
    {
        // image bar filled area
        float proportion = value / max;
        if (proportion < 0)
            proportion = 0.0f;
        imageBar.fillAmount = proportion;

        // text value
        if (value < 0)
            value = 0;
        textValue.text = value.ToString();

        // text max value
        textValueMax.text = "/" + max.ToString();
    }
}
