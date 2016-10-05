using UnityEngine;
using System.Collections;

public abstract class SfxBase : MonoBehaviour {

    [Header("Sfx")]
    public AudioClip Clip_OnStart;
    public AudioClip Clip_Loop;
    public AudioClip Clip_OnDestroy;

    protected AudioManager Audio;
    protected int AudioSourceLoopIndex;

    protected virtual void Start()
    {
        // Get audio manager
        GameManager gameMgr = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        Audio = gameMgr.AudioManager;
            
        // Play sfx on start
        if (Clip_OnStart)
            Audio.PlaySfx(Clip_OnStart);

        // Play looping sdx
        if (Clip_Loop)
            AudioSourceLoopIndex = Audio.PlaySfx(Clip_Loop, true);
    }

    protected virtual void OnDestroy()
    {
        // Stop looping sfx
        if (Clip_Loop)
            Audio.StopAudioSource(AudioSourceLoopIndex);
    }

    public virtual void destroy()
    {
        // Play onDestroy sfx
        if (Clip_OnDestroy)
            Audio.PlaySfx(Clip_OnDestroy);
    }
}
