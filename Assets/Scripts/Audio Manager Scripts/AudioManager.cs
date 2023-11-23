using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    private AudioSource audioSource;

    void Awake()
    {
        var instance = Instance; // Accessing the instance to trigger DontDestroyOnLoad
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu" || scene.name == "CharacterSelect")
        {
            PlayMusic(menuMusic);
        }
        else if (scene.name == "MultiplayerTest")
        {
            PlayMusic(gameMusic);
        }
    }

    void PlayMusic(AudioClip clip)
    {
        if (audioSource.clip != clip || !audioSource.isPlaying)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}

