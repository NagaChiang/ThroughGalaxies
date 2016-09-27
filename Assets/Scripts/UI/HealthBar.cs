using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBar : Bar {

    public Text textValue;
    public Text textValueMax;
    public Text textLife;

    public override void update(float value, float max)
    {
        // update filled bar
        base.update(value, max);

        // update filled bars color
        updateBarColor(value, max);

        // update text
        if(textValue)
            updateText(value, max);
    }

    public void updateLife(int life)
    {
        if(textLife)
            textLife.text = life.ToString();
    }

    private void updateBarColor(float value, float max)
    {
        float proportion = value / max;
        Color newColor;

        // >50%: green
        if (proportion >= 0.5f)
            newColor = Color.green;

        // 25% ~ 50%: yellow
        else if (proportion >= 0.25f && proportion < 0.5f)
            newColor = Color.yellow;

        // <25%: red
        else
            newColor = Color.red;

        // assign to bars
        imageBar.color = newColor;
        imageBarBehind.color = newColor / 2;
    }

    private void updateText(float value, float max)
    {
        // text value
        if (value < 0)
            value = 0;
        textValue.text = value.ToString();

        // text max value
        textValueMax.text = "/" + max.ToString();
    }
}
