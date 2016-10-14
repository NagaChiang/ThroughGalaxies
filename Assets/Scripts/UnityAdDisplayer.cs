using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;
using System;

public class UnityAdDisplayer : MonoBehaviour {

    public string GameID;
    public bool EnabledTestMode;

    void Start()
    {
        if (Advertisement.isSupported)
        {
            Advertisement.Initialize(GameID, EnabledTestMode);
        }
    }

	public void ShowAd(Action<ShowResult> callback)
    {
        // Ad options
        ShowOptions options = new ShowOptions();
        options.resultCallback = callback;

        // Show
        Advertisement.Show(options);
    }
}
