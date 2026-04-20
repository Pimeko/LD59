using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        StartCoroutine(LoadGameAsync());
    }

    IEnumerator LoadGameAsync()
    {
#if UNITY_ADDRESSABLES_EXIST
        // Iterate all the asset references and start loading their studio banks
        // in the background, including their audio sample data
        foreach (var bank in AssetReferenceBanks)
        {
            FMODUnity.RuntimeManager.LoadBank(bank, true);
        }
#else
        // Iterate all the Studio Banks and start them loading in the background
        // including the audio sample data
        FMODUnity.RuntimeManager.LoadBank("Music", true);
#endif

        // Keep yielding the co-routine until all the bank loading is done
        // (for platforms with asynchronous bank loading)
        while (!FMODUnity.RuntimeManager.HaveAllBanksLoaded)
        {
            yield return null;
        }

        // Keep yielding the co-routine until all the sample data loading is done
        while (FMODUnity.RuntimeManager.AnySampleDataLoading())
        {
            yield return null;
        }

        musicInstance = RuntimeManager.CreateInstance(fmodEventPath);
        musicInstance.setCallback(MusicCallback,
            FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
    }

    public void StartMusic()
    {
        //while (true)
        //{
        //    if (FMODUnity.RuntimeManager.HasBankLoaded("Music"))
        //        break;
        //}
        musicInstance.start();
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

    public bool IsLastMusic()
    {
        return musicIndex == nbMusics - 1;
    }

    public void NextMusic()
    {
        musicIndex++;
        musicInstance.setParameterByName("MusicIndex", musicIndex);
    }

    void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }
}
