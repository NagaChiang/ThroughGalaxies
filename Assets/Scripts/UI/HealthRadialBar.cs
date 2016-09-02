using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthRadialBar : RadialBar {

    public float delayedTime;
    public float lerpTime;
    public Image imageDelayedBar;
    public Text textValue;
    public Text textValueMax;

    public new void update(float value, float max)
    {
        // update filled bar
        base.update(value, max);
        StartCoroutine(updateDelayedBar(value, max));

        // update filled bars color
        updateBarColor(value, max);

        // update text
        updateText(value, max);
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
        imageDelayedBar.color = new Color(newColor.r, newColor.g, newColor.b,
                                            imageDelayedBar.color.a);
    }

    private IEnumerator updateDelayedBar(float value, float max)
    {
        float proportion = value / max;
        if (proportion < 0)
            proportion = 0.0f;

        // delay
        yield return new WaitForSeconds(delayedTime);

        // lerp to new value
        float lastProportion = imageDelayedBar.fillAmount;
        for (float time = 0.0f; time < lerpTime; time += Time.deltaTime)
        {
            imageDelayedBar.fillAmount = Mathf.Lerp(lastProportion, proportion, time / lerpTime);
            yield return null;
        }
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
