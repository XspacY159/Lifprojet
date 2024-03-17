using TMPro;
using UnityEngine;

public class PlayerTeamController : TeamController
{
    [Header("Ressources")]
    [SerializeField] private int ressourcesCount;
    [SerializeField] private int maxRessources;
    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI aliveUnitsText;
    [SerializeField] private TextMeshProUGUI ressourcesText;

    private void Update()
    {
        aliveUnitsText.text = "Units Alive : " + units.Count;

        ressourcesText.text = "Ressources : " + ressourcesCount + "/" + maxRessources;
    }

    public int AddRessources(int ressources)
    {
        int remainingAmount = 0;
        if(ressourcesCount + ressources > maxRessources)
        {
            remainingAmount = ressources - (maxRessources - ressourcesCount);
        }

        return remainingAmount;
    }

    public int GetRessourcesCount() 
    { 
        return ressourcesCount;
    }

    public int GetMaxRessources() 
    {
        return maxRessources;
    }
}
