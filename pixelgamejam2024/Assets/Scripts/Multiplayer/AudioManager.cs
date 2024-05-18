using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioManager Instance;
    private AudioSource _audioSource;
    private AudioClip _clip;

    
    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void SetClip(AudioClip clip)
    {
        _clip = clip;
    }

    public void PlayClip()
    {
        //Audio playing logic
    }
}
