using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    #region SINGLETON
    public static MusicController Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion

    [SerializeField]
    string fmodEventPath;
    [SerializeField]
    int nbMusics;

    public enum MusicEffect
    {
        Volume,
        Distortion,
        Reverb,
        Flanger,
        Pitch,
    }

    public static Action OnBeat;

    EventInstance musicInstance;
    int musicIndex;

    void Start()
    {
        musicIndex = 0;

        musicInstance = RuntimeManager.CreateInstance(fmodEventPath);
        musicInstance.setCallback(MusicCallback,
            FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
        DOVirtual.DelayedCall(1, () =>
        {
            musicInstance.start();
        });
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT MusicCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr _event, IntPtr parameters)
    {
        if (type == FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT)
        {
            OnBeat?.Invoke();
        }

        return FMOD.RESULT.OK;
    }

    public void ChangeValue(MusicEffect effect, float value)
    {
        musicInstance.setParameterByName(effect.ToString(), value);
    }

    public void NextMusic()
    {
        musicIndex++;
        if (musicIndex == nbMusics)
        {
            print("FINISH!!!");
        }
        else
        {
            musicIndex %= nbMusics;
            musicInstance.setParameterByName("MusicIndex", musicIndex);
        }
    }

    void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }
}
