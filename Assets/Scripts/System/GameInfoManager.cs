using UnityEngine;

public class GameInfoManager : MonoBehaviour
{
    public static GameInfoManager Instance;

    [SerializeField] private TeamName playerTeam;

    private void OnEnable()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void SetPlayerTeam(TeamName team)
    {  
        playerTeam = team; 
    }

    public TeamName GetPlayerTeam() 
    {
        return playerTeam; 
    }
}
