using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class Bar : MonoBehaviour {

    public float delayedTime;
    public float lerpTime;
    public Image imageDelayedBar;
    public Image imageBar;

    public void update(float value, float max)
    {
        // filled area of image bar
        updateFilledBar(value, max);
        StartCoroutine(updateDelayedBar(value, max));
    }

    private void updateFilledBar(float value, float max)
    {
        // image bar filled area
        float proportion = value / max;
        if (proportion < 0)
            proportion = 0.0f;
        imageBar.fillAmount = proportion;
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
}
