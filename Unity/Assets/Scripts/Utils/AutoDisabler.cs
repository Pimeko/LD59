using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisabler : MonoBehaviour
{
    private void OnEnable()
    {
        gameObject.SetActive(false);
    }
}