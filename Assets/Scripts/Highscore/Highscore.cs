using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Highscore : MonoBehaviour {

    public HerokuDatabase Database;
    public ScoreEntry[] ScoreEntries;

    private const int NUM_SCORE_DIGIT = 10;

    public IEnumerator UpdateScores(List<NameScoreData> scoreData)
    {
        // Wait for data downloading
        while (scoreData.Count == 0)
            yield return null;

        // Update
        for(int i = 0; i < scoreData.Count; i++)
        {
            // Assign scores to entries
            ScoreEntries[i].Name.text = scoreData[i].Name;
            ScoreEntries[i].Score.text = scoreData[i].Score.ToString();
        }
    }

    private string CompensatePrefixZeros(int num, int totalLength)
    {
        string str = num.ToString();
        while (totalLength - str.Length > 0)
            str = "0" + str;

        return str;
    }
}
