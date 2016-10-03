using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct PlayerData
{
    public int Score;
    public int Stage;
    public int BoltLevel;
    public int SphereLevel;
    public int LaserLevel;
}

public struct NameScoreData
{
    public string Name;
    public int Score;
}

public class HerokuDatabase : MonoBehaviour {

    [HideInInspector]
    public int NOT_HIGHSCORE = -1;
    [HideInInspector]
    public int ERROR_STATE = -2;
    [HideInInspector]
    public int BUSY_STATE = -3;

    private int CurrentRowId;

    private const string UrlSubmitPlayerData = "https://through-galaxies.herokuapp.com/SubmitPlayerData.php";
    private const string UrlSubmitHighscore = "https://through-galaxies.herokuapp.com/SubmitHighscore.php";
    private const string UrlGetHighscore = "https://through-galaxies.herokuapp.com/GetHighscore.php";

    // Use callback function to return id if it's a new highscore; otherwise, return -1
    public IEnumerator SubmitPlayerData(PlayerData data, Action<int> callback)
    {
        callback(BUSY_STATE);

        // Prepare post data
        WWWForm form = new WWWForm();
        form.AddField("score", data.Score);
        form.AddField("stage", data.Stage);
        form.AddField("boltLevel", data.BoltLevel);
        form.AddField("sphereLevel", data.SphereLevel);
        form.AddField("laserLevel", data.LaserLevel);

        // Connect to database
        WWW www = new WWW(UrlSubmitPlayerData, form);
        yield return www;

        // Check connection
        if(www.error != null)
        {
            // Log the error message
            Debug.LogWarning(www.error);
            callback(ERROR_STATE);
        }
        else
        {
            // Pass the result back
            try
            {
                int parsed = int.Parse(www.text);
                CurrentRowId = parsed;
                callback(parsed);
            }
            catch (FormatException)
            {
                Debug.LogError("Bad format: " + www.text);
            }         
        }
    }

    // Update name to current row id
    public IEnumerator SubmitHighscoreName(string name, Action<bool> callback)
    {
        // Busy
        callback(false);

        // Prepare post data
        WWWForm form = new WWWForm();
        form.AddField("id", CurrentRowId);
        form.AddField("name", name);

        // Connect to database
        WWW www = new WWW(UrlSubmitHighscore, form);
        yield return www;

        // Check connection
        if (www.error != null)
        {
            // Log the error message
            Debug.LogError(www.error);
        }

        // Finished
        callback(true);
    }

    public IEnumerator GetHighscoreData(List<NameScoreData> data)
    {
        // Connect to database
        WWW www = new WWW(UrlGetHighscore);
        yield return www;

        // Check connection
        if (www.error != null)
        {
            // Log the error message
            Debug.LogError(www.error);

            // Pass dummy back to continue
            NameScoreData dummy = new NameScoreData();
            data.Add(dummy);
        }
        else
        {
            // Extract data
            data.AddRange(ExtractNameScoreData(www.text));
        }
    }

    private List<NameScoreData> ExtractNameScoreData(string text)
    {
        List<NameScoreData> extractedData = new List<NameScoreData>();
        StringReader reader = new StringReader(text);
        string line = reader.ReadLine();
        while(line != null)
        {
            NameScoreData data = new NameScoreData();

            // Name
            data.Name = line;

            // Score
            try
            {
                line = reader.ReadLine();
                data.Score = int.Parse(line);
            }
            catch(FormatException)
            {
                Debug.LogError("Bad format: " + line);
            }

            // Add to list
            extractedData.Add(data);

            // Read a line for next iteration
            line = reader.ReadLine();
        }

        return extractedData;
    }
}
