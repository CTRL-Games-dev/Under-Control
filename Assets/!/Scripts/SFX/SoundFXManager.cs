using System;
using Unity.Mathematics;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    [SerializeField] private AudioSource soundFXObject;
    public static SoundFXManager Instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if(Instance==null){
            Instance = this;
        }
    }
    public void PlaySoundFXClip(AudioClip audioClip, Transform _transform, float custompitch = 1f, float volume = 1f, float speed = 1f){
        if(_transform ==null){
            _transform = transform;
        }
        AudioSource audioSource = Instantiate(soundFXObject, _transform.position , Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        if(custompitch != 1f){
            audioSource.pitch = custompitch;
        }else{
            audioSource.pitch = UnityEngine.Random.Range(0.95f,1.05f);
        }
        audioSource.Play();

        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }
}
