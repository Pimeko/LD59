using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
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

    public enum MusicEffect
    {
        Distortion,
        Reverb,
        Flanger,
        Pitch,
    }

    EventInstance eventInstance;

    void Start()
    {
        eventInstance = RuntimeManager.CreateInstance(fmodEventPath);
        DOVirtual.DelayedCall(1, () =>
        {
            eventInstance.start();
        });
    }

    public void ChangeValue(MusicEffect effect, float value)
    {
        eventInstance.setParameterByName(effect.ToString(), value);
    }

    void OnDestroy()
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        eventInstance.release();
    }
}
