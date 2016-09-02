using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public Wave[] waves;
    public Text guiTextScore;
    public int numScoreDigit;

    private int _score;
    private float _difficultyFactor;

	void Start ()
    {
        // initial properties
        _score = 0;
        _difficultyFactor = 1.0f;

        // update score UI
        updateScoreUI(_score);

        // spawn waves
        StartCoroutine(spawnWaves());
    }

    public void addScore(int num)
    {
        // add to total score (with difficulty bonus)
        _score += (int)(num * _difficultyFactor);

        // update score UI
        updateScoreUI(_score);
    }

    public void gameover()
    {

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

    private void updateScoreUI(int newScore)
    {
        // compensate prefix 0s
        string strScore = newScore.ToString();
        while (numScoreDigit - strScore.Length > 0)
            strScore = "0" + strScore;

        // update to GUI
        guiTextScore.text = strScore;
    }
}
