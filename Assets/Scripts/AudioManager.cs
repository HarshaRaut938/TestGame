using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SoundEffect
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0.1f, 3f)]
    public float pitch = 1f;
}

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("AudioManager");
                    instance = go.AddComponent<AudioManager>();
                }
            }
            return instance;
        }
    }

    [Header("Sound Effects")]
    [SerializeField] private SoundEffect[] soundEffects;
    
    [Header("Audio Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float masterVolume = 1f;

    private AudioSource audioSource;
    private Dictionary<string, SoundEffect> soundDictionary;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeAudio();
    }

    private void InitializeAudio()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        soundDictionary = new Dictionary<string, SoundEffect>();
        foreach (var sound in soundEffects)
        {
            if (sound.clip == null)
            {
                continue;
            }
            soundDictionary[sound.name] = sound;
        }
    }

    public void PlaySound(string soundName)
    {
        if (!soundDictionary.TryGetValue(soundName, out SoundEffect sound))
        {
            return;
        }

        if (sound.clip == null)
        {
             return;
        }

        audioSource.clip = sound.clip;
        audioSource.volume = sound.volume * masterVolume;
        audioSource.pitch = sound.pitch;
        audioSource.loop = false;
        audioSource.Play();
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        if (audioSource.isPlaying)
        {
            audioSource.volume = audioSource.volume * masterVolume;
        }
    }

    private void OnValidate()
    {
        if (soundEffects != null)
        {
            HashSet<string> names = new HashSet<string>();
            foreach (var sound in soundEffects)
            {
                if (!string.IsNullOrEmpty(sound.name))
                {
                    if (!names.Add(sound.name))
                    {
                        Debug.LogError($"AudioManager: Duplicate sound name found: '{sound.name}'");
                    }
                }
            }
        }
    }
} 