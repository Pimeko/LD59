using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomJukeboxController : MonoBehaviour
{
    [SerializeField]
    Animator animator;

    void Start()
    {
        MusicController.OnBeat += OnBeat;
    }

    void OnBeat()
    {
        animator.SetTrigger("beat");
    }

    void OnDestroy()
    {
        MusicController.OnBeat -= OnBeat;
    }
}
