using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    public Dictionary<GameObject, UnitGeneral> unitsList { get; private set; }
        = new Dictionary<GameObject, UnitGeneral>(); //Dictionnary of all units

    [SerializeField] private List<TeamController> teamControllers = new List<TeamController>();

    private void OnEnable()
    {
        if(Instance == null)
            Instance = this;
    }

    public void AddUnit(UnitGeneral unit)
    {
        if (!unitsList.ContainsKey(unit.gameObject))
            unitsList.Add(unit.gameObject, unit);

        foreach (TeamController team in teamControllers)
        {
            if (team.GetTeamName() == unit.GetTeam())
                team.AddUnit(unit);
        }
    }

    public void RemoveUnit(UnitGeneral unit)
    {
        if (unitsList.ContainsKey(unit.gameObject))
            unitsList.Remove(unit.gameObject);

        foreach (TeamController team in teamControllers)
            team.RemoveUnit(unit);
    }

    public UnitGeneral GetUnit(GameObject go)
    {
        if (!unitsList.ContainsKey(go)) return null;

        return unitsList[go];
    }

    public List<TeamController> GetTeamControllers()
    {
        return teamControllers;
    }
}
