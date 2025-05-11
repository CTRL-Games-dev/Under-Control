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

    private EventInstance attackInstance;

    public static AudioManager instance {get; private set;}
    private void Awake(){
        if(instance != null){
            Debug.LogError("More than one AudioManager in the scene");
        }
        instance = this;

        eventInstances = new List<EventInstance>();
    }

    private void Start(){
        InitializeMusic(FMODEvents.instance._MusicPlayer);
    }

    private void InitializeMusic(EventReference eventReference){
        musicInstance = RuntimeManager.CreateInstance(eventReference);
        musicInstance.start();
    }
    private void InitializeAmbience(EventReference eventReference){
        ambientInstance = RuntimeManager.CreateInstance(eventReference);
        ambientInstance.start();
    } 

    public void setMusicArea(MusicArea area){
        musicInstance.setParameterByName("Area", (float )area);
    }

    public void PlayOneShot(EventReference sound, Vector3 position){
        RuntimeManager.PlayOneShot(sound, position);
    }

    public void PlayAttackSound(EventReference sound, Vector3 position, WeaponType weaponType)
    {
        EventInstance attackInstance = RuntimeManager.CreateInstance(sound);
        attackInstance.setParameterByName("WeaponType", (float)weaponType);
        attackInstance.start();
        attackInstance.release();
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
