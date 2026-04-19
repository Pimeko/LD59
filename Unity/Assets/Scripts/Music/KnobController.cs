using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnobController : MonoBehaviour
{
    [SerializeField]
    MusicController.MusicEffect effect;
    [SerializeField]
    Slider slider;

    void Start()
    {
        slider.onValueChanged.AddListener(OnValueChanged);
    }

    void OnValueChanged(float value)
    {
        MusicController.Instance.ChangeValue(effect, value);
    }

    void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(OnValueChanged);
    }
}
