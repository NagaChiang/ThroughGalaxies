using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour {

    public Wave[] waves;
    public Text guiTextScore;
    public int numScoreDigit;
    public ExpCrystal[] expCrystals;

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

    public void dropExperience(Vector3 center, float radius, int exp)
    {
        // sort the crystals from small to big
        //Array.Sort(expCrystals); // TODO: IComparable

        // random generate the combinations of crystals
        while(exp >= expCrystals[0].experience)
        {
            // find the index of the biggest crystal suitable
            int indexMax = 0;
            for(int i = 0; i < expCrystals.Length; i++)
            {
                if(exp >= expCrystals[i].experience)
                    indexMax = i;
            }

            // random suitable crystal
            ExpCrystal crystal = expCrystals[UnityEngine.Random.Range(0, indexMax + 1)];
            exp -= crystal.experience;

            // random position within a circle
            Vector2 posInUnitCircle = UnityEngine.Random.insideUnitCircle;
            Vector3 position = center + new Vector3(posInUnitCircle.x, 0.0f, posInUnitCircle.y);

            // instantiate game object
            Instantiate(crystal, position, crystal.transform.rotation);
        }
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