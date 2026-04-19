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
    public bool IsVolumeOK { get { return volumeValue.IsInRange(validMarginVolume); } }
    public bool IsEffectOK { get { return effectValue.IsInRange(validMarginEffect); } }
    public bool IsPitchTooSlow { get { return pitchValue < validMarginPitch.x; } }
    public bool IsPitchTooFast { get { return pitchValue > validMarginPitch.y; } }
    public bool IsPitchOK { get { return pitchValue.IsInRange(validMarginPitch); } }

    void Start()
    {
        volumeKnob.OnKnobChange += OnVolumeChange;
        effectKnob.OnKnobChange += OnEffectChange;
        pitchKnob.OnKnobChange += OnPitchChange;
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

    void OnPitchChange(KnobController _, float value)
    {
        pitchValue = value;
        CheckAllKnobs();
    }

    void CheckAllKnobs()
    {
        if (IsVolumeOK && IsEffectOK && IsPitchOK)
        {
            OnAllKnobsOK?.Invoke();
        }
    }

    void OnDestroy()
    {
        volumeKnob.OnKnobChange -= OnVolumeChange;
        effectKnob.OnKnobChange -= OnEffectChange;
        pitchKnob.OnKnobChange -= OnPitchChange;
    }
}
