using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class Bar : MonoBehaviour {

    public float delayedTime;
    public float lerpTime;
    public Image imageDelayedBar;
    public Image imageBar;

    private bool isLerping;

    protected void Start()
    {
        // initial properties
        isLerping = false;
    }

    public void update(float value, float max)
    {
        // filled area of image bar
        updateFilledBar(value, max);
        updateDelayedBar(value, max);
    }

    private void updateFilledBar(float value, float max)
    {
        float lastProportion = imageBar.fillAmount;
        float proportion = value / max;
        if (proportion < 0)
            proportion = 0.0f;

        // delayed effect when being healed
        if (proportion >= lastProportion)
            StartCoroutine(lerpImageFilledAmountTo(imageBar, proportion));
        else
            imageBar.fillAmount = proportion;
    }

    private void updateDelayedBar(float value, float max)
    {
        float lastProportion = imageDelayedBar.fillAmount;
        float proportion = value / max;
        if (proportion < 0)
            proportion = 0.0f;

        // delayed effect when being damaged
        if (proportion < lastProportion)
            StartCoroutine(lerpImageFilledAmountTo(imageDelayedBar, proportion));
        else
            imageDelayedBar.fillAmount = proportion;
    }

    private IEnumerator lerpImageFilledAmountTo(Image image, float valueNew)
    {
        // delay
        yield return new WaitForSeconds(delayedTime);

        // wait for unfinished lerping
        while (isLerping)
            yield return null;

        // get value again
        float valueLast = image.fillAmount;

        // lerp to new value
        isLerping = true;
        for (float time = 0.0f; time < lerpTime; time += Time.deltaTime)
        {
            image.fillAmount = Mathf.Lerp(valueLast, valueNew, time / lerpTime);
            yield return null;
        }

        // end the lerping
        isLerping = false;
    }
}
