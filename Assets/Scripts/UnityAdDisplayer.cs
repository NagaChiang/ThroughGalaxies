using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

public class UnityAdDisplayer : MonoBehaviour {

    public string GameID;
    public bool EnabledTestMode;

    public delegate void ResultCallBack(ShowResult result);
    public ResultCallBack Callback;

    void Start()
    {
        if (Advertisement.isSupported)
        {
            Advertisement.Initialize(GameID, EnabledTestMode);
        }
    }

	public void ShowAd()
    {

    }

    private IEnumerator DoShowAd()
    {
        // Wait for the advertisement ready
        while (!Advertisement.isInitialized || !Advertisement.IsReady())
        {
            yield return new WaitForSeconds(0.5f);
        }

        // Ad options
        ShowOptions options = new ShowOptions();
        //options.resultCallback = Callback;
    }
}
