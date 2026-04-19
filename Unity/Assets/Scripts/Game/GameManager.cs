using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region SINGLETON
    public static GameManager Instance;

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

    public Transform characterZone;

    [SerializeField]
    KnobController volumeKnob, effectKnob, pitchKnob;
    [SerializeField]
    Vector2 validMarginVolume, validMarginEffect, validMarginPitch;

    public Action OnAllKnobsOK;

    float volumeValue, effectValue, pitchValue;

    void Start()
    {
        volumeKnob.OnKnobChange += OnVolumeChange;
        effectKnob.OnKnobChange += OnEffectChange;
        pitchKnob.OnKnobChange += OnVPitchChange;
    }

    void OnVolumeChange(KnobController _, float value)
    {
        volumeValue = value;
        CheckAllKnobs();
    }

    void OnEffectChange(KnobController _, float value)
    {
        effectValue = value;
        CheckAllKnobs();
    }

    void OnVPitchChange(KnobController _, float value)
    {
        pitchValue = value;
        CheckAllKnobs();
    }

    void CheckAllKnobs()
    {
        if (volumeValue.IsInRange(validMarginVolume)
            && effectValue.IsInRange(validMarginEffect)
            && pitchValue.IsInRange(validMarginPitch))
        {
            OnAllKnobsOK?.Invoke();
        }
    }

    void OnDestroy()
    {
        volumeKnob.OnKnobChange -= OnVolumeChange;
        effectKnob.OnKnobChange -= OnEffectChange;
        pitchKnob.OnKnobChange -= OnVPitchChange;
    }
}
