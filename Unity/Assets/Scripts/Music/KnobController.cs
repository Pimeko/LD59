using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class KnobController : MonoBehaviour
{
    [SerializeField]
    MusicController.MusicEffect effect;

    [SerializeField]
    bool useRandomEffect = false;
    [SerializeField]
    List<MusicController.MusicEffect> effects;
    [SerializeField]
    KnobRotateController knobRotateController;
    [SerializeField]
    List<Vector2> rangeBad;

    float randomOffset;
    int randomEffectIndex;

    public Action<KnobController, float> OnKnobChange;
    public float Value;

    void Start()
    {
        knobRotateController.OnValueChanged += OnValueChanged;
        GameManager.Instance.OnNextMusic += NextMusic;
        GenerateRandomOffset();
        randomEffectIndex = 0;

        DOVirtual.DelayedCall(0.5f, () =>
        {
            GenerateRandomOffset();
        });
    }

    void NextMusic()
    {
        if (useRandomEffect)
        {
            MusicController.Instance.ChangeValue(effect, 0);
            randomEffectIndex++;
            randomEffectIndex %= effects.Count;
            effect = effects[randomEffectIndex];
        }
        GenerateRandomOffset();
    }

    void GenerateRandomOffset()
    {
        randomOffset = UnityEngine.Random.Range(0, 360);
        var randomRange = rangeBad.GetRandomItem();
        float badValue = UnityEngine.Random.Range(randomRange.x, randomRange.y);
        badValue += randomOffset;
        badValue %= 360;
        OnValueChanged(badValue);
    }

    void OnValueChanged(float value)
    {
        value += randomOffset;
        value %= 360;

        float newValue = -1;
        if (value > 180)
            newValue = value.ChangeRange(new Vector2(180f, 360f), new Vector2(1f, 0f));
        else
            newValue = value.ChangeRange(new Vector2(0f, 180f), new Vector2(0f, 1f));
        //print(value + " " + newValue);
        MusicController.Instance.ChangeValue(effect, newValue);

        OnKnobChange?.Invoke(this, newValue);
    }

    void OnDestroy()
    {
        knobRotateController.OnValueChanged -= OnValueChanged;
        GameManager.Instance.OnNextMusic -= NextMusic;
    }
}
