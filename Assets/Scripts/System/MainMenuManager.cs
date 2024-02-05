using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : LevelManager
{
    [SerializeField] private GameObject eventManager;

    [Header("Settings Menu")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundEffectSlider;

    //[SerializeField] private Slider cameraShakeSlider;
    //[SerializeField] private TextMeshProUGUI writingSpeedButtonText;
    //[SerializeField] private TextMeshProUGUI pauseOnLostFocusButtonText;

    [Header("Auto Scene Load")]
    [SerializeField] private bool autoLoad;
    [SerializeField] private string sceneToLoad;

    public override void LevelStart()
    {
        masterSlider.onValueChanged.AddListener(val => SoundManager.Instance.ChangeMasterVolume(val));
        musicSlider.onValueChanged.AddListener(val => SoundManager.Instance.ChangeMusicVolume(val));
        soundEffectSlider.onValueChanged.AddListener(val => SoundManager.Instance.ChangeEffectVolume(val));
        //cameraShakeSlider.onValueChanged.AddListener(val => ChangeCameraShakeGlobalForce(val));

        masterSlider.value = GameManager.Instance.GetGameSettings().masterVolume;
        musicSlider.value = GameManager.Instance.GetGameSettings().musicVolume;
        soundEffectSlider.value = GameManager.Instance.GetGameSettings().soundEffectsVolume;
        //cameraShakeSlider.value = GameManager.Instance.GetGameSettings().globalCameraShakeForce;

        if (autoLoad)
        {
            GameManager.Instance.LoadLevel(sceneToLoad);
        }

        Cursor.visible = true;
    }

    public void PlayGame(string scene)
    {
        eventManager.SetActive(false);
        GameManager.Instance.LoadLevel(scene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
