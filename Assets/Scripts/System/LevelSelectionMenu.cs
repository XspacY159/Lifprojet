public class LevelSelectionMenu : LevelManager
{
    public void LoadLevel(string sceneName)
    {
        GameManager.Instance.LoadLevel(sceneName);
    }
}
