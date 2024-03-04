using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using InputProvider.Graph;

public class EnnemyBTSolver : MonoBehaviour
{
    [SerializeField]
    private IPTreeReader tree;
    [SerializeField]
    private UnitGeneral unit;
    private UnitGeneral targetUnit;
    [SerializeField]
    private POI targetObjective;
    private Vector3 targetPosition;

    void Start()
    {
        tree.SwitchStateKey("combatAggressive", "noAdversary");
        tree.SwitchStateKey("defending", "waiting");
        tree.SwitchStateKey("moveToObjective", "onTheMove");
        targetUnit = null;
    }

    void Update()
    {
        if (UnitSelectionController.Instance == null) return;
        switch (unit.baseState)
        {
            case AIState.Aggressive:
                tree.SwitchStateKey("state", "aggressive");
                break;
            case AIState.Defensive:
                tree.SwitchStateKey("state", "defensive");
                break;
            case AIState.FollowObjective:
                tree.SwitchStateKey("state", "followObjective");
                break;
        }
        solve();
    }

    public void solve()
    {
        string currentNode = tree.ResolveGraph();

        if(targetUnit == null)
        {
            foreach (Collider unitInRange in unit.UnitsInRange())
            {
                if (unitInRange.gameObject == unit.gameObject) continue;
                //UnitGeneral tmp = unitInRange.gameObject.GetComponent<UnitGeneral>();
                UnitGeneral tmp = UnitSelectionController.Instance.unitsList[unitInRange.gameObject];
                if (tmp.GetTeam() != unit.GetTeam()) //Beware! GetComponent could be a performance problem!
                {
                    targetUnit = tmp;
                    break;
                }
            }
        }
        

        switch (currentNode)
        {
            // == AI state is aggressive
            case "idleAggressive":
                //unit waits for an ennemy
                if (targetUnit != null)
                {
                    tree.SwitchStateKey("combatAggressive", "adversaryFound");
                }
                break;

            case "attackAggressive":
                //unit attacks
                unit.AttackUnit(targetUnit);
                if (targetUnit == null || Vector3.Distance(unit.transform.position, targetUnit.transform.position) > unit.GetStats().attackRange + 0.7)
                {
                    tree.SwitchStateKey("combatAggressive", "noAdversary");
                    targetUnit = null;
                }
                break;

            // == AI state is defensive
            case "defend":
                //unit waits for an ennemy
                targetPosition = unit.transform.position;
                if (targetUnit != null)
                {
                    tree.SwitchStateKey("defending", "fighting");
                    tree.SwitchStateKey("combatDefense", "adversaryFound");
                }
                break;

            case "backToDefense":
                //the unit decided to stop its fight
                unit.GoTo(targetPosition);
                if(Vector3.Distance(unit.transform.position, targetPosition) < 0.5)
                {
                    tree.SwitchStateKey("defending", "waiting");
                }
                break;

            case "attackDefense":
                //if not too far from its objective, unit attacks
                unit.AttackUnit(targetUnit);
                if(Vector3.Distance(unit.transform.position, targetPosition) > 2.0)
                {
                    tree.SwitchStateKey("combatDefense", "noAdversary");
                    targetUnit = null;
                }
                break;


            // == AI state is followObjective
            case "move":
                //unit follows its pathfinding to the specified objective
                targetPosition = targetObjective.transform.position;
                unit.GoTo(targetPosition);
                if (Vector3.Distance(unit.transform.position, targetPosition) < 0.5)
                {
                    tree.SwitchStateKey("moveToObjective", "arrived");
                    break;
                }
                if (targetUnit != null)
                {
                    targetPosition = unit.transform.position;
                    tree.SwitchStateKey("moveToObjective", "fighting");
                    tree.SwitchStateKey("combatObjective", "adversaryFound");
                }
                break;
            case "backToObjective":
                //the unit decided to stop its fight
                unit.GoTo(targetPosition);
                if (Vector3.Distance(unit.transform.position, targetPosition) < 0.5)
                {
                    tree.SwitchStateKey("moveToObjective", "onTheMove");
                }
                break;
            case "attackObjective":
                //unit attacks
                unit.AttackUnit(targetUnit);
                if (Vector3.Distance(unit.transform.position, targetPosition) > 2.0)
                {
                    tree.SwitchStateKey("combatObjective", "noAdversary");
                    targetUnit = null;
                }
                break;
            case "changeState":
                //unit arrived to its destination, and switches state
                tree.SwitchStateKey("state","defensive");
                break;

        }

        //Debug.Log(currentNode);
    }
}
