using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public Wave[] waves;

    private float _difficultyFactor;

	void Start ()
    {
        // initial properties
        _difficultyFactor = 1.0f;

        // spawn waves
        StartCoroutine(spawnWaves());
    }

    private IEnumerator spawnWaves()
    {
        // spawn waves one by one
        foreach(Wave wave in waves)
        {
            if (wave != null)
            {
                StartCoroutine(wave.spawn(_difficultyFactor));
                yield return new WaitForSeconds(wave.duration);
            }
        }
    }
}
