using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class Bar : MonoBehaviour {

    public float delayedTime;
    public float lerpTime;
    public Image imageBarBehind;
    public Image imageBar;

    private float _lastUpdateTime = 0.0f;
    private float _currentValue = 0.0f;

    void OnEnable()
    {
        // Replenish instantly
        _currentValue = 1.0f;
        imageBar.fillAmount = 1.0f;
        imageBarBehind.fillAmount = 1.0f;
    }

    void Update()
    {
        // instant update depending on increasing or decreasing
        if(_currentValue > imageBarBehind.fillAmount)
        {
            // increasing
            imageBarBehind.fillAmount = _currentValue;
        }
        else if(_currentValue < imageBar.fillAmount)
        {
            // decreasing
            imageBar.fillAmount = _currentValue;
        }

        // delay effect for filled amount updating
        if((Time.time - _lastUpdateTime) >= delayedTime)
        {
            // bar front
            lerpBarToValue(imageBar, _currentValue);

            // bar behind
            lerpBarToValue(imageBarBehind, _currentValue);
        }
    }

    public virtual void update(float value, float max)
    {
        // record the update time stamp
        _lastUpdateTime = Time.time;

        // filled area of image bar
        float proportion = value / max;
        if (proportion < 0)
            proportion = 0.0f;

        // update true value
        _currentValue = proportion;
    }

    private void lerpBarToValue(Image bar, float destValue)
    {
        // move gradually
        float diff = _currentValue - bar.fillAmount;
        bar.fillAmount += diff * (Time.deltaTime / lerpTime);

        // threshold to complement little gap
        float threshold = 0.025f;
        if (Mathf.Abs(_currentValue - bar.fillAmount) < threshold)
            bar.fillAmount = _currentValue;
    }
}
