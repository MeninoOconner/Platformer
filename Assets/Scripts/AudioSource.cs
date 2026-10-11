using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Música")]
    public AudioClip backgroundMusic;
    [Range(0f, 1f)]
    public float musicVolume = 0.4f;

    [Header("Efeitos")]
    public AudioClip jumpClip;
    public AudioClip coinClip;
    public AudioClip hurtClip;
    public AudioClip winClip;
    public AudioClip gameOverClip;
    [Range(0f, 1f)]
    public float sfxVolume = 0.8f;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
    }

    void Start()
    {
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }

    public void PlayJump()
    {
        PlaySfx(jumpClip);
    }

    public void PlayCoin()
    {
        PlaySfx(coinClip);
    }

    public void PlayHurt()
    {
        PlaySfx(hurtClip);
    }

    public void PlayWin()
    {
        PlaySfx(winClip);
    }

    public void PlayGameOver()
    {
        PlaySfx(gameOverClip);
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    void PlaySfx(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;

        sfxSource.PlayOneShot(clip, sfxVolume);
    }
}