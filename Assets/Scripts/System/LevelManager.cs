using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Level Params")]
    public static LevelManager CurrentInstance;

    [SerializeField] private AudioClip musicToPlay;
    private void OnEnable()
    {
        if (CurrentInstance == null)
        {
            CurrentInstance = this;
        }
    }

    protected virtual void LevelStart()
    {

    }

    void Start()
    {
        if(musicToPlay != null)
            SoundManager.Instance.PlayMusic(musicToPlay, true);

        LevelStart();
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        SoundManager.Instance.PlaySoundEffect(clip);
    }
}
