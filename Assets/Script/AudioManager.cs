using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
public class audioManager : MonoBehaviour
{
    public static audioManager instance;
    [SerializeField] private AudioSource source, lastPlayedSFX;
    [SerializeField] private AudioSource mainMusic;
    public AudioMixerGroup sfx, music;
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    public AudioSource playAudio(AudioClip a, float volume, float pitch, Transform pos, AudioMixerGroup channel)
    {
        if (a == null) return null;

        AudioSource audioSource = Instantiate(source, pos.position, Quaternion.identity);
        audioSource.clip = a;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.outputAudioMixerGroup = channel;
        audioSource.Play();
        float len = audioSource.clip.length;
        Destroy(audioSource.gameObject, len);
        return audioSource;
    }
    public AudioSource playRandomAudio(AudioClip[] a, float volume, float pitch, Transform pos, AudioMixerGroup channel)
    {
        if (a == null) return null;

        AudioSource audioSource = Instantiate(source, pos.position, Quaternion.identity);
        audioSource.clip = a[Random.Range(0, a.Length)];
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.outputAudioMixerGroup = channel;
        audioSource.Play();
        float len = audioSource.clip.length;
        Destroy(audioSource.gameObject, len);
        return audioSource;
    }



    public void stopMusic()
    {
        mainMusic.Stop();
    }
    public void startMusic()
    { mainMusic.Play(); }
}
