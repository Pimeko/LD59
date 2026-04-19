using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JukeboxController : MonoBehaviour
{
    [SerializeField]
    List<KnobController> knobControllers;

    void Start()
    {
        foreach (var controller in knobControllers)
        {
            controller.OnKnobChange += OnKnobChange;
        }
    }

    void OnKnobChange(KnobController controller)
    {
        controller.transform.SetAsLastSibling();
    }

    void OnDestroy()
    {
        foreach (var controller in knobControllers)
        {
            controller.OnKnobChange -= OnKnobChange;
        }
    }
}
