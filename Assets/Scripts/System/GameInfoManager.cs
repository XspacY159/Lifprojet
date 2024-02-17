using UnityEngine;

public class GameInfoManager : MonoBehaviour
{
    public static GameInfoManager Instance;

    [SerializeField] private Team playerTeam;

    private void OnEnable()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void SetPlayerTeam(Team team)
    {  
        playerTeam = team; 
    }

    public Team GetPlayerTeam() 
    {
        return playerTeam; 
    }
}
