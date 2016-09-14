﻿using System;
using System.Collections.Generic;
using UnityEngine;
using ModU3DToolkit.Core;

[AddComponentMenu("ModU3DToolkit/Managers/SoundsManager")]
public class SoundsManager : MonoBehaviour
{
    #region Singleton instance
    protected static SoundsManager instance = null;

    public static SoundsManager Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    #region Public members
    public AudioClip musicClip = null;
    public bool pauseMusic = false;
    public bool useLevelRootToFindSources = true;
    public bool useAlternativeBehavior = false;
    #endregion


    public float MusicVolume
    {
        get { return musicVol; }
        set
        {
            musicVol = value;
            UpdateMusicOnOff();
        }
    }
    float musicVol = 1f;


    #region Protected members
    protected bool musicOn = true;
    protected bool soundsOn = true;
    protected bool deviceMusicPlaying = false;
    protected Dictionary<AudioSource, bool> wasPlaying = new Dictionary<AudioSource, bool>();
    #endregion

    #region Public properties
    public bool MusicActive
    {
        get
        {
            return musicOn;
        }
        set
        {
            if (value != musicOn)
                this.ToggleMusicOnOff();
        }
    }

    public bool SoundsActive
    {
        get
        {
            return soundsOn;
        }
        set
        {
            if (value != soundsOn)
                this.ToggleSoundsOnOff();
        }
    }
    #endregion

    #region Virtual functions
    protected virtual void ToggleMusicOnOff()
    {
        musicOn = !musicOn;
        PlayerPrefs.SetInt("sm_mu", musicOn ? 1 : 0);

        UpdateMusicOnOff();
    }

    protected void UpdateMusicOnOff()
    {
        if (null == musicClip)// || (pauseMusic && TimeManager.Instance.MasterSource.IsPaused))
            return;

        IEnumerable<AudioSource> sources = null;

        Transform[] transforms = (Transform[])Transform.FindSceneObjectsOfType(typeof(Transform));

        List<AudioSource> sourceList = new List<AudioSource>();
        sources = sourceList;
        foreach (Transform tr in transforms)
        {
            if (null == tr.parent)
            {
                AudioSource[] subSources = tr.gameObject.GetComponentsInChildren<AudioSource>(true);
                foreach (AudioSource source in subSources)
                    sourceList.Add(source);
            }
        }


        foreach (AudioSource source in sources)
        {
            if (source.clip == musicClip)
            {
                source.volume = musicVol;
#if UNITY_IPHONE && !UNITY_EDITOR
                source.mute = !musicOn || deviceMusicPlaying;
#else
                source.mute = !musicOn;
#endif
            }
        }
    }

    protected virtual void ToggleSoundsOnOff()
    {
        soundsOn = !soundsOn;
        PlayerPrefs.SetInt("sm_sf", soundsOn ? 1 : 0);

        IEnumerable<AudioSource> sources = null;

        Transform[] transforms = (Transform[])Transform.FindSceneObjectsOfType(typeof(Transform));

        List<AudioSource> sourceList = new List<AudioSource>();
        sources = sourceList;
        foreach (Transform tr in transforms)
        {
            if (null == tr.parent)
            {
                AudioSource[] subSources = tr.gameObject.GetComponentsInChildren<AudioSource>(true);
                foreach (AudioSource source in subSources)
                    sourceList.Add(source);
            }
        }


        foreach (AudioSource source in sources)
        {
            if (null == source.clip || source.clip != musicClip)
            {
                source.mute = !soundsOn;
            }
        }
    }

    public virtual void PlayClick()
    {
    }
    #endregion

    #region Public functions
    public void PlayMusicSource(AudioSource source)
    {
        source.Play();
        if (TimeManager.Instance.MasterSource.IsPaused)
        {
            if (pauseMusic)
                source.Pause();
        }
        if (!musicOn)
            source.mute = true;
        else
        {
#if UNITY_IPHONE && !UNITY_EDITOR
            if (deviceMusicPlaying)
                source.mute = true;
#endif
        }
    }

    public void StopMusicSource()
    {
        if (null == musicClip)
            return;

        IEnumerable<AudioSource> sources = null;

        Transform[] transforms = (Transform[])Transform.FindSceneObjectsOfType(typeof(Transform));

        List<AudioSource> sourceList = new List<AudioSource>();
        sources = sourceList;
        foreach (Transform tr in transforms)
        {
            if (null == tr.parent)
            {
                AudioSource[] subSources = tr.gameObject.GetComponentsInChildren<AudioSource>(true);
                foreach (AudioSource source in subSources)
                    sourceList.Add(source);
            }
        }


        foreach (AudioSource source in sources)
        {
            if (source.clip == musicClip)
            {
                source.Stop();
            }
        }

        musicClip = null;
    }

    public void PlaySource(AudioSource source, bool bForced = false)
    {
        if (useAlternativeBehavior)
        {
            if (TimeManager.Instance.MasterSource.IsPaused == false)
                source.Play();
        }
        else
        {
            source.Play();
        }

        if (TimeManager.Instance.MasterSource.IsPaused && !bForced)
        {
            if (wasPlaying.ContainsKey(source))
                wasPlaying[source] = source.isPlaying;
            else
                wasPlaying.Add(source, source.isPlaying);
            source.Pause();
        }
        if (!soundsOn)
            source.mute = true;
    }

    public void StopSource(AudioSource source)
    {
        source.Stop();
        if (TimeManager.Instance.MasterSource.IsPaused)
        {
            if (wasPlaying.ContainsKey(source))
                wasPlaying[source] = source.isPlaying;
            else
                wasPlaying.Add(source, source.isPlaying);
        }
    }

    public void UpdateDeviceMusicPlaying()
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        deviceMusicPlaying = UtilsBindings.IsMusicPlaying();
#endif
        UpdateMusicOnOff();
    }
    #endregion

    #region Messages
    void OnPause()
    {
        AudioSource[] sources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        foreach (AudioSource source in sources)
        {
            if (source.clip == musicClip)
            {
                if (pauseMusic)// && musicOn)
                    source.Pause();
            }
            else/* if (soundsOn)*/
            {
                if (wasPlaying.ContainsKey(source))
                    wasPlaying[source] = source.isPlaying;
                else
                    wasPlaying.Add(source, source.isPlaying);
                source.Pause();
            }
        }
    }

    void OnResume()
    {
        AudioSource[] sources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        foreach (AudioSource source in sources)
        {
            if (source.clip == musicClip)
            {
                if (pauseMusic)// && musicOn)
                    source.Play();
            }
            else/* if (soundsOn)*/
            {
                bool playAgain = true;
                wasPlaying.TryGetValue(source, out playAgain);
                if (playAgain)
                    source.Play();
            }
        }
    }
    #endregion

    #region Unity Callbacks
    void Awake()
    {
        Asserts.Assert(null == instance);
        instance = this;

#if UNITY_IPHONE && !UNITY_EDITOR
        deviceMusicPlaying = UtilsBindings.IsMusicPlaying();
#endif
        musicOn = PlayerPrefs.GetInt("sm_mu", 1) == 1;
        soundsOn = PlayerPrefs.GetInt("sm_sf", 1) == 1;
    }

    void OnDestroy()
    {
        Asserts.Assert(this == instance);
        instance = null;
    }


    void OnApplicationPause(bool paused)
    {
        bool bValue = !paused && TimeManager.Instance.MasterSource.IsPaused;

        if (useAlternativeBehavior)
            bValue = paused || TimeManager.Instance.MasterSource.IsPaused;

        if (useAlternativeBehavior)
        {
            AudioSource[] sources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
            foreach (AudioSource source in sources)
            {
                if (source.clip == musicClip)
                {
                    if (pauseMusic)
                        source.Pause();
                }
                else
                {
                    source.Pause();
                }
            }
        }
    }
    #endregion
}
