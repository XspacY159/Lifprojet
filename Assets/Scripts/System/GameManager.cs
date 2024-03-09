using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private Image crossFaderImage;
    [SerializeField] private float crossFadeSpeed;

    private float interpolation;

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
        //DataPercistenceManager.Instance.LoadGame();
    }

    #region Load Level Management
    public void LoadLevel(string sceneName)
    {
        StartCoroutine(SoundManager.Instance.FadeMusicOut(1));
        StartCoroutine(LoadLevelCo(sceneName));
    }

    public void LoadLevel(Scene scene)
    {
        StartCoroutine(SoundManager.Instance.FadeMusicOut(1));
        StartCoroutine(LoadLevelCo(scene.name));
    }

    private IEnumerator LoadLevelCo(string _sceneName)
    {
        crossFaderImage.gameObject.SetActive(true);
        interpolation = 0;

        while (interpolation < crossFadeSpeed)
        {
            if (crossFadeSpeed != 0)
                interpolation += Time.unscaledDeltaTime;
            else
                interpolation = 1;

            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
            crossFaderImage.color = new Color(crossFaderImage.color.r,
                crossFaderImage.color.g, crossFaderImage.color.b, interpolation / crossFadeSpeed);
        }
        SceneManager.LoadScene(_sceneName);
    }

    private void OnSceneChanged(Scene current, Scene next)
    {
        //PauseSystem.Instance.SetPause(false);
        //gameoverMenu.SetActive(false);
        //pauseMenu.SetActive(false);
        //insideMenu = false;

        FadeOut();
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutCo());
    }
    private IEnumerator FadeOutCo()
    {
        //if (Player.Instance != null)
        //    playerInputProvider.SetKeyState("transitionRestriction", true);

        //ClosePauseMenu();
        interpolation = crossFadeSpeed;
        while (interpolation > 0)
        {
            if (crossFadeSpeed != 0)
                interpolation -= Time.unscaledDeltaTime;
            else
                interpolation = 1;

            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
            crossFaderImage.color = new Color(crossFaderImage.color.r,
                crossFaderImage.color.g, crossFaderImage.color.b, interpolation / crossFadeSpeed);
        }

        crossFaderImage.gameObject.SetActive(false);
        //if (Player.Instance != null)
        //    playerInputProvider.SetKeyState("transitionRestriction", false);
    }

    public void LoadMainMenu()
    {
        LoadLevel("MainMenu");
    }

    public void ReloadCurrentScene()
    {
        LoadLevel(SceneManager.GetActiveScene().name);
    }
    #endregion

    public void TriggerGameOver()
    {
        //TODO
        Debug.Log("Lost");
    }

    public GameSettings GetGameSettings()
    {
        return gameSettings;
    }

    public void SetGameSettings(GameSettings settings)
    {
        gameSettings = settings;
    }
}

[Serializable]
public class GameSettings
{
    public float masterVolume;

    public float musicVolume;

    public float soundEffectsVolume;

    public float defaultWritingSpeed;

    public float globalCameraShakeForce;

    public bool pauseGameOnLostFocus;

    public GameSettings()
    {
        defaultWritingSpeed = 0.05f;

        masterVolume = 0.5f;

        musicVolume = 0.5f;

        soundEffectsVolume = 0.5f;

        globalCameraShakeForce = 1;

        pauseGameOnLostFocus = false;
    }
}
