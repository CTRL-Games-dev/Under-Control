using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour {
    public AudioClip[] MusicClips;
    public int CurrentClipIndex = 0;
    public bool RandomizeOrder = true;

    private AudioSource _audioSource;
    private bool _shouldPlay = false;

    void Awake() {
        _audioSource = GetComponent<AudioSource>();

        StartCoroutine(runAudioLooper());
    }

    public void Stop() {
        _shouldPlay = false;
        _audioSource.Stop();
    }

    public void Play() {
        _shouldPlay = true;
    }

    private IEnumerator runAudioLooper() {
        while(true) {
            yield return new WaitUntil(() => _shouldPlay);

            _audioSource.Stop();
            _audioSource.clip = MusicClips[CurrentClipIndex];
            _audioSource.Play();

            yield return new WaitUntil(() => _audioSource.isPlaying == false);

            if(RandomizeOrder) {
                CurrentClipIndex = Random.Range(0, MusicClips.Length);
            } else {
                CurrentClipIndex = (CurrentClipIndex + 1) % MusicClips.Length;
            }
        }
    }
}