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
    public static Dictionary<string, float> Clips = new Dictionary<string, float>();
    public static Dictionary<string, float> clipSavedVolume = new Dictionary<string, float>();

    private const float lowestSoundValue = 0.3f;

    [SerializeField] AudioClip[] musicList = null;
    [SerializeField] private bool restartMusicOnLoad = false;

    public bool softMusicStop = true;

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
        softMusicPause = softMusicStop; // from static to non-static
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
        if (softMusicStop && musicPaused) // soft music fade-out
        {
            audioSourceMusic.volume -= Time.deltaTime * savedPauseVolume;
            if (audioSourceMusic.volume <= 0)
            {
                audioSourceMusic.Pause();
            }
        }
    }

    void MusicCheck() 
    { // checking if music is correct and changing it if needed        
        int expectedMusicIndex = 0; // index for array musicList, 0 is for no music
        String sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "MainMenu") { expectedMusicIndex = 2; } //logic for music selection 
        else if (sceneName.Contains("BossOne")) { expectedMusicIndex = 0; }
        else if (sceneName.Contains("Tutorial")) { expectedMusicIndex = 1; }
        else if (sceneName.Contains("Chapter2") || sceneName.Contains("Boss2")) { expectedMusicIndex = 4; }
        else { expectedMusicIndex = 3; }

        audioSourceMusic = transform.GetChild(0).GetComponent<AudioSource>();

        if (expectedMusicIndex == 0){
            audioSourceMusic.Stop();
        } else if (!audioSourceMusic.isPlaying || audioSourceMusic.clip.name != musicList[expectedMusicIndex].name)
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
            float ltp = Clips[name];
            volume = clipSavedVolume[name];
            volume = Mathf.Lerp(volume / 5, volume, Time.time - ltp); //sound suppression for multiple identical effects in short time
            Clips[name] = Time.time;
        }
        else
        {
            Clips.Add(name, Time.time);
            clipSavedVolume.Add(name, volume);
        }
        return volume * userPrefSound;
    }

    public static void Play(string name, AudioSource source)
    {
        float volume = 0;
#if UNITY_WEBGL
        source.spatialBlend = 0;
        volume = GetVolume(name, source.volume / 3);
#else
        volume = GetVolume(name, source.volume);
#endif
        if (CharacterLife.isDeath == true)
        {
            volume /= 2;
        }
        source.PlayOneShot(source.clip, userPrefSound * volume);
    }

    public static void Play(AudioClip clip)
    {
        if (!clip) return;

        audioSourceSFX.clip = clip;
        Play(clip.name, audioSourceSFX);
    }

    public static void PauseSource(string name, AudioSource source)
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
        audioSourceMusic.volume = musicVolume;
    }

    public static void PlayMusic(AudioSource sorce, float time = 0)// for externall audio sorce with music volume, like on boss
    {
        sorce.volume = savedVolume * userPrefMusic;
        sorce.time = time;
        sorce.spatialBlend = 0;
        audioSourceMusic = sorce;
        sorce.Play();
        musicPaused = false;
    }

    private static ulong savedTime = 0;
    public static void PauseMusic()
    {
        if (audioSourceMusic)
        {
            musicPaused = true;
            if (!softMusicPause) audioSourceMusic.Pause();
            savedTime = (ulong)audioSourceMusic.time; // это не работает
            savedPauseVolume = audioSourceMusic.volume;
        }
    }

    public static void ResumeMusic()
    {
        if (audioSourceMusic)
        {
            musicPaused = false;
            audioSourceMusic.volume = savedPauseVolume;
            if (!audioSourceMusic.isPlaying)
            {
                audioSourceMusic.Play(savedTime);
            }
        }
    }

    private static bool musicPaused = false;
    private static float savedVolume;
    private static float savedPauseVolume;
    private static bool softMusicPause = true;
}
