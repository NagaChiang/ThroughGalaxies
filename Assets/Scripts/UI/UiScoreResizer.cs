using UnityEngine;
using System.Collections;

public class UiScoreResizer : MonoBehaviour {

    public Vector3 ChangedScale;
    public float RecoverTime;

    private Vector3 InitialScale;
    private float StartTime;

    void Start()
    {
        InitialScale = transform.localScale;
    }

    void Update()
    {
        // Gradually recover to initial scale
        float threshold = 0.001f;
        float diff = Mathf.Abs(Vector3.SqrMagnitude(transform.localScale - InitialScale));
        if (diff > threshold)
        {
            // Recover
            float t = (Time.time - StartTime) / RecoverTime;
            transform.localScale = Vector3.Lerp(ChangedScale, InitialScale, t);
        }
        else
        {
            // Within threshold, set it back to initial scale
            transform.localScale = InitialScale;
        }
    }

    public void ChangeScale()
    {
        StartTime = Time.time;
        transform.localScale = ChangedScale;
    }
}
