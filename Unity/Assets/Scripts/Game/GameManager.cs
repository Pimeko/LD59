using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField]
    float allIsOKMinDuration, allIsOKDuration;
    [SerializeField]
    GameObject victoryGameObject;
    [SerializeField]
    TMP_Text victoryNb;

    public Action OnAllKnobsOK, OnAllKnobsNotOK, OnNextMusic, OnFinish;

    float volumeValue, effectValue, pitchValue;
    public bool IsVolumeOK { get { return volumeValue.IsInRange(validMarginVolume); } }
    public bool IsEffectOK { get { return effectValue.IsInRange(validMarginEffect); } }
    public bool IsPitchTooSlow { get { return pitchValue < validMarginPitch.x; } }
    public bool IsPitchTooFast { get { return pitchValue > validMarginPitch.y; } }
    public bool IsPitchOK { get { return pitchValue.IsInRange(validMarginPitch); } }
    public bool IsAllOK { get { return IsVolumeOK && IsEffectOK && IsPitchOK; } }

    bool reachedAllIsOkMin, allIsOK;
    float allIsOKMinElapsed, allIsOKElapsed;

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
        if (IsAllOK)
        {
            allIsOK = true;
            allIsOKElapsed = 0;
        }
        else if (allIsOK)
        {
            OnAllKnobsNotOK?.Invoke();
            reachedAllIsOkMin = false;
            allIsOK = false;
        }
    }

    public void NextMusic()
    {
        //print("Next music");
        MusicController.Instance.NextMusic();
        OnNextMusic?.Invoke();
        allIsOK = false;
        reachedAllIsOkMin = false;
    }

    public void Finish()
    {
        victoryGameObject.SetActive(true);
        victoryNb.text = RoomCharactersController.Instance.characterControllers.Count.ToString();
        OnFinish?.Invoke();
    }

    void Update()
    {
        if (allIsOK)
        {
            if (!reachedAllIsOkMin)
            {
                allIsOKMinElapsed += Time.deltaTime;
                if (allIsOKMinElapsed > allIsOKMinDuration)
                {
                    OnAllKnobsOK?.Invoke();
                    reachedAllIsOkMin = true;
                }
            }
            allIsOKElapsed += Time.deltaTime;
            if (allIsOKElapsed > allIsOKDuration)
            {
                NextMusic();
            }
        }
    }

    void OnDestroy()
    {
        volumeKnob.OnKnobChange -= OnVolumeChange;
        effectKnob.OnKnobChange -= OnEffectChange;
        pitchKnob.OnKnobChange -= OnPitchChange;
    }
}
