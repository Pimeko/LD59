using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnobController : MonoBehaviour
{
    [SerializeField]
    MusicController.MusicEffect effect;
    [SerializeField]
    KnobRotateController knobRotateController;

    float randomOffset;

    void Start()
    {
        knobRotateController.OnValueChanged += OnValueChanged;
        GenerateRandomOffset();
    }

    void GenerateRandomOffset()
    {
        randomOffset = Random.Range(0, 360);
        DOVirtual.DelayedCall(0.5f, () =>
        {
            OnValueChanged(0);
        });
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
    }

    void OnDestroy()
    {
        knobRotateController.OnValueChanged -= OnValueChanged;
    }
}
