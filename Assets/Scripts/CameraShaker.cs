using UnityEngine;
using System.Collections;

// Note: This only allows one shake at a time.
//          Latter shake would overwrite the former one.

[RequireComponent(typeof(Camera))]
public class CameraShaker : MonoBehaviour {

    private Vector3 OriginalPosition;
    private float ShakeIntensity;
    private float ShakeEndTime;
    private float DampingDuration;
    private float Damping;

    void Start()
    {
        // Store the original position
        OriginalPosition = transform.position;
    }

    void Update()
    {
        // During shake duration
        if(Time.time <= ShakeEndTime)
        {
            // Reset damping factor
            Damping = 1.0f;

            // Shake
            Vector3 offset = GenerateShakeOffset(ShakeIntensity);
            transform.position = OriginalPosition + offset;
        }
        else if(Time.time <= ShakeEndTime + DampingDuration)
        {
            // Calculate damping factor
            Damping -= Time.deltaTime / DampingDuration;

            // Shake with damping
            Vector3 offset = GenerateShakeOffset(ShakeIntensity, Damping);
            transform.position = OriginalPosition + offset;
        }
        else
        {
            // Set back to original position
            transform.position = OriginalPosition;
        }
    }

    public void SetShake(float intensity, float duration, float dampingDuration)
    {
        // Set values to private variables to allow shaking in Update()
        ShakeIntensity = intensity;
        ShakeEndTime = Time.time + duration;
        DampingDuration = dampingDuration;
    }

    private Vector3 GenerateShakeOffset(float intensity, float damping = 1.0f)
    {
        // Use Perlin noise
        float sampleX = Mathf.PerlinNoise(Time.time, 0.0f) * GetRandomSign();
        float sampleY = Mathf.PerlinNoise(0.0f, Time.time) * GetRandomSign();

        // Generate offset
        Vector3 offset = new Vector3(sampleX * intensity * damping, 0.0f,
                                        sampleY * intensity * damping);
        return offset;
    }
    private float GetRandomSign()
    {
        return Random.value < 0.5 ? 1.0f : -1.0f;
    }
}
