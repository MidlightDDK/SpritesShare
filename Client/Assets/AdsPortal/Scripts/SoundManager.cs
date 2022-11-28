using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] sounds;

    // Internal
    private AudioSource speakers;
    private float pitchVariation = 0.1f;

    public static SoundManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        speakers = gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    public void PlaySound(int soundIndex)
    {
        if (soundIndex >= 0 && soundIndex < sounds.Length)
        {
            speakers.pitch = 1.0f + Random.Range(-pitchVariation, pitchVariation);
            speakers.PlayOneShot(sounds[soundIndex]);
        }
    }

    // Update is called once per frame
    public void PlaySoundFixPitch(int soundIndex)
    {
        if (soundIndex >= 0 && soundIndex < sounds.Length)
        {
            speakers.pitch = 1.0f;
            speakers.PlayOneShot(sounds[soundIndex]);
        }
    }
}
