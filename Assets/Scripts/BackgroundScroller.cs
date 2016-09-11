using UnityEngine;
using System.Collections;

public class BackgroundScroller : MonoBehaviour {

    public float scrollSpeed;
    public float scrollSpeedBoost;
    public float boostDuration;
    public float backgroundHeight;

    public bool isBoostEnabled { get; set; }

    private float _speed;
    private float _tBoost;

	void Start ()
    {
        isBoostEnabled = false;
        _speed = scrollSpeed;
        _tBoost = 0.0f;
    }
	
	void FixedUpdate ()
    {
        // calculate speed
        _speed = Mathf.Lerp(scrollSpeed, scrollSpeedBoost, _tBoost);

        // lerp step t
        if (isBoostEnabled)
            _tBoost += Time.deltaTime / boostDuration;
        else
            _tBoost -= Time.deltaTime / boostDuration;
        _tBoost = Mathf.Clamp(_tBoost, 0.0f, 1.0f);

        // assign z position
        Vector3 lastPos = transform.position;
        float newZ = Mathf.Repeat(-lastPos.z + Time.deltaTime * _speed, backgroundHeight);
        transform.position = new Vector3(lastPos.x, lastPos.y, -newZ);
    }
}
