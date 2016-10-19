using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour {

    [Range(0.0f, 1.0f)]
    public float BgmVolume;
    [Range(0.0f, 1.0f)]
    public float SfxVolume;
    [Range(0.0f, 1.0f)]
    public float SfxOneShotVolume;
    public AudioSource BgmSource;
    public AudioSource[] SfxSources;

    [Header("Audio Clips")]
    public AudioClip Clip_Bgm;
    public AudioClip Clip_ButtonClick;
    public AudioClip Clip_GalaxyIntro;

    [Header("UI")]
    public Slider SliderMusic;
    public Slider SliderSfx;

    void Start()
    {
        // Initialize values of UI
        SliderMusic.value = BgmVolume;
        SliderSfx.value = SfxVolume;
    }

    public void OnSliderValueChanged()
    {
        BgmVolume = SliderMusic.value;
        BgmSource.volume = BgmVolume;
        SfxVolume = SliderSfx.value;
    }

    public void PlayBgm(AudioClip clip)
    {
        BgmSource.clip = clip;
        BgmSource.loop = true;
        BgmSource.volume = BgmVolume;
        BgmSource.Play();
    }

    public int PlaySfx(AudioClip clip, bool isLoop = false)
    {
        // Find an idle source to play it
        int index = GetIdleSfxSourceIndex();
        if (index >= 0 && index < SfxSources.Length)
        {
            AudioSource source = SfxSources[index];
            source.clip = clip;
            source.pitch = Random.Range(0.98f, 1.02f);
            source.loop = isLoop;
            source.volume = SfxVolume;
            source.Play();
        }

        // All busy; use one shot instead
        if(!isLoop)
        {
            index = GetOneshotSfxSourceIndex();
            if (index >= 0 && index < SfxSources.Length)
            {
                AudioSource source = SfxSources[index];
                source.PlayOneShot(clip, SfxOneShotVolume * SfxVolume);
            }
        }


        // Return index
        return index;
    }

    public void StopAudioSource(int index)
    {
        if (index >= 0 && index < SfxSources.Length)
        {
            if(SfxSources[index])
                SfxSources[index].Stop();
        }
    }

    private int GetIdleSfxSourceIndex()
    {
        // Find an idle audio source
        for(int i = 0; i < SfxSources.Length; i++)
        {
            if (!SfxSources[i].isPlaying)
                return i;
        }

        // All sources are busy
        return -1;
    }

    private int GetOneshotSfxSourceIndex()
    {
        // Find an idle audio source
        for (int i = 0; i < SfxSources.Length; i++)
        {
            if (!SfxSources[i].loop)
                return i;
        }

        // All sources are busy
        return -1;
    }
}
