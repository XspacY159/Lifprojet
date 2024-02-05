using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource musicSource, effectSource;

    private AudioClip musicToLoop;

    bool fadingOut = false;

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        //Music Loop
        if (musicToLoop == null) return;

        if (!musicSource.isPlaying)
            musicSource.PlayOneShot(musicToLoop);

        RefreshVolumes();
    }

    private void RefreshVolumes()
    {
        if (fadingOut) return;
        //Main menu audio volume triggers
        if (AudioListener.volume != GameManager.Instance.GetGameSettings().masterVolume)
        {
            AudioListener.volume = GameManager.Instance.GetGameSettings().masterVolume;
        }

        if (musicSource.volume != GameManager.Instance.GetGameSettings().musicVolume)
        {
            musicSource.volume = GameManager.Instance.GetGameSettings().musicVolume;
        }

        if (effectSource.volume != GameManager.Instance.GetGameSettings().soundEffectsVolume)
        {
            effectSource.volume = GameManager.Instance.GetGameSettings().soundEffectsVolume;
        }
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        effectSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip, bool loop)
    {
        musicSource.PlayOneShot(clip);
        if (loop)
            musicToLoop = clip;
        else
            musicToLoop = null;
    }

    public IEnumerator FadeMusicOut(float fadeTime, AudioClip nextMusicToPlay = null, bool loopNextMusic = false)
    {
        float interpolation = fadeTime;
        float musicVolume = musicSource.volume;
        fadingOut = true;

        while (interpolation > 0)
        {
            if (fadeTime != 0)
                interpolation -= Time.unscaledDeltaTime;
            else
                interpolation = 1;

            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);


            musicSource.volume = musicVolume * interpolation / fadeTime;
        }

        musicSource.Stop();
        musicToLoop = null;
        musicSource.volume = GameManager.Instance.GetGameSettings().musicVolume;
        fadingOut = false;

        if (nextMusicToPlay != null)
            PlayMusic(nextMusicToPlay, loopNextMusic);
    }

    public void StopSound(AudioSource audioSource)
    {
        audioSource.Stop();
    }

    public void ChangeMasterVolume(float value)
    {
        GameSettings newSettings = GameManager.Instance.GetGameSettings();
        newSettings.masterVolume = value;
        GameManager.Instance.SetGameSettings(newSettings);
    }

    public void ChangeMusicVolume(float value)
    {
        GameSettings newSettings = GameManager.Instance.GetGameSettings();
        newSettings.musicVolume = value;
        GameManager.Instance.SetGameSettings(newSettings);
    }

    public void ChangeEffectVolume(float value)
    {
        GameSettings newSettings = GameManager.Instance.GetGameSettings();
        newSettings.soundEffectsVolume = value;
        GameManager.Instance.SetGameSettings(newSettings);
    }
}
