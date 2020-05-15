using UnityEngine.Audio;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public static AudioSource audioSourceSFX;
    public static AudioSource audioSourceMusic;

    private static float userPrefSound = 0.5f;
    public static float userPrefMusic { get; private set; } = 0.5f;

    // Name -> (time since last sound, maximum value)
    public static Dictionary<string, Vector2> Clips = new Dictionary<string, Vector2>();

    private const float lowestSoundValue = 0.3f;

    [SerializeField]
    private AudioSource SourceMusic = null; // duplicate of static for inspector

    [SerializeField]
    AudioClip[] musicList = null;
    [SerializeField]
    private bool restartMusicOnLoad = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private Scene lastFrameScene;

    private void Start()
    {
        if (PlayerPrefs.HasKey("SoundVolume"))
        {
            userPrefSound = PlayerPrefs.GetFloat("SoundVolume");
        }
        else
        {
            userPrefSound = 0.5f;
            PlayerPrefs.SetFloat("SoundVolume", 0.5f);
        }

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            userPrefMusic = PlayerPrefs.GetFloat("MusicVolume");
        }
        else
        {
            userPrefMusic = 0.5f;
            PlayerPrefs.SetFloat("MusicVolume", 0.5f);
        }

        audioSourceSFX = GetComponent<AudioSource>();
        audioSourceMusic = transform.GetChild(0).GetComponent<AudioSource>();
        savedVolume = audioSourceMusic.volume;
        if (audioSourceMusic != null)
        {
            SetVolumeMusic(userPrefMusic);
            MusicCheck();
        }
    }

    private void Update()
    {
        Scene newScene = SceneManager.GetActiveScene();
        if (lastFrameScene != newScene) { MusicCheck(); } // if scene changed
        lastFrameScene = newScene;
    }

    void MusicCheck() 
    { // checking if music is correct and changing it if needed        
        int expectedMusicIndex = 0; // index for array musicList, 0 is for no music
        String sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "MainMenu") { expectedMusicIndex = 2; } //logic for music selection 
        else if (sceneName.Contains("boss")) { expectedMusicIndex = 0; }
        else { expectedMusicIndex = 1; }

        audioSourceMusic = transform.GetChild(0).GetComponent<AudioSource>();

        if (expectedMusicIndex == 0){
            audioSourceMusic.Stop();
        } else if (!audioSourceMusic.isPlaying || audioSourceMusic.clip != musicList[expectedMusicIndex])
        {
            audioSourceMusic.Stop();
            audioSourceMusic.clip = musicList[expectedMusicIndex];
            PlayMusic(audioSourceMusic);
        } else if (restartMusicOnLoad)
        {
            audioSourceMusic.Stop();
            PlayMusic(audioSourceMusic);
        }
    }

    public static float GetVolume(string name, float volume)
    {
        if (Clips.ContainsKey(name))
        {
            float ltp = Clips[name].x;
            volume = Mathf.Lerp(Clips[name].y / 5, Clips[name].y, Time.time - ltp); //sound suppression for multiple identical effects in short time
            Clips[name] = new Vector2(Time.time, Clips[name].y);
        }
        else
        {
            Clips.Add(name, new Vector2(Time.time, volume));
        }
        return volume * userPrefSound;
    }

    public static void Play(string name, AudioSource source)
    {
#if UNITY_WEBGL
        source.spatialBlend = 0;
        source.volume = GetVolume(name, source.volume / 3);
#else
        source.volume = GetVolume(name, source.volume);
#endif
        if (CharacterLife.isDeath == true)
        {
            source.volume = source.volume / 2;
        }
        source.Play();
    }

    public static void Pause(string name, AudioSource source)
    {
        if (source != null)
        {
            source.Pause();
        }
    }

    public static bool isPlaying(string name, AudioSource source)
    {
        if (source.isPlaying)
            return true;
        else
            return false;
    }

    public static void SetVolumeSFX(float value)
    {
        userPrefSound = value;
#if UNITY_WEBGL
        audioSourceSFX.volume = userPrefSound / 3f;
#else
        audioSourceSFX.volume = userPrefSound;
#endif
    }

    public static void SetVolumeMusic(float value)
    {
        var musicVolume = value;
#if UNITY_WEBGL
        audioSourceMusic.volume = musicVolume / 3f;
#else
        audioSourceMusic.volume = musicVolume;
#endif
    }

    public static void PlayMusic(AudioSource sorce, float time = 0)// for externall audio sorce with music volume, like on boss
    {
        sorce.volume = savedVolume * userPrefMusic;
        sorce.time = time;
        sorce.spatialBlend = 0;
        audioSourceMusic = sorce;
        sorce.Play();
    }

    private static ulong savedTime = 0;
    public static void PauseMusic()
    {
        audioSourceMusic.Pause();
        savedTime = (ulong)audioSourceMusic.time;
    }

    public static void ResumeMusic()
    {
        audioSourceMusic.Play();
    }

    private static float savedVolume;
}
