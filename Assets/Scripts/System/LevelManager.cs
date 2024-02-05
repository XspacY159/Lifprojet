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

    public virtual void LevelStart()
    {

    }

    void Start()
    {
        if(musicToPlay != null)
            SoundManager.Instance.PlayMusic(musicToPlay, true);

        LevelStart();
    }

    private void Update()
    {
        //if (generateDungeonLevel && !PauseSystem.Instance.GetPauseState())
        //{
        //    if (Cursor.visible == true)
        //        Cursor.visible = false;
        //}
        //else
        //    Cursor.visible = true;
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        SoundManager.Instance.PlaySoundEffect(clip);
    }
}
