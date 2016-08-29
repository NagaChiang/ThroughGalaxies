using UnityEngine;
using System.Collections;

public class BackgroundScroller : MonoBehaviour {

    public float scrollSpeed;
    public float backgroundHeight;

    private Vector3 _positionStart;

	void Start ()
    {
        _positionStart = transform.position;
    }
	
	void Update ()
    {
        float newZ = Mathf.Repeat(Time.time * scrollSpeed, backgroundHeight);
        transform.position = _positionStart + Vector3.back * newZ;
    }
}
