using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    private List<EventInstance> eventInstances;

    private EventInstance ambientInstance;

    private EventInstance musicInstance;

    public static AudioManager instance {get; private set;}
    private void Awake(){
        if(instance != null){
            Debug.LogError("More than one AudioManager in the scene");
        }
        instance = this;

        eventInstances = new List<EventInstance>();
    }

    private void Start(){
        InitializeAmbience(FMODEvents.instance._ambience);
        InitializeAmbience(FMODEvents.instance._MusicPlayer);
    }

    private void InitializeMusic(EventReference eventReference){
        musicInstance = RuntimeManager.CreateInstance(eventReference);
        musicInstance.start();
    }
    private void InitializeAmbience(EventReference eventReference){
        ambientInstance = RuntimeManager.CreateInstance(eventReference);
        ambientInstance.start();
    } 

    public void PlayOneShot(EventReference sound, Vector3 position){
        RuntimeManager.PlayOneShot(sound, position);
    }

    public EventInstance CreateEventInstance(EventReference eventReference){
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    private void CleanUp(){
        foreach(EventInstance eventInstance in eventInstances){
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }

    private void OnDestroy(){
        CleanUp();
    }
}
