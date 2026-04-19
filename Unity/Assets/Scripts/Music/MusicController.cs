using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField]
    string fmodEventPath;

    EventInstance eventInstance;

    void Start()
    {
        eventInstance = RuntimeManager.CreateInstance(fmodEventPath);
        eventInstance.start();
    }

    void OnDestroy()
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        eventInstance.release();
    }
}
