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
    private Vector3 objective;

    void Start()
    {
        tree.SwitchStateKey("combatAggressive", "noAdversary");
        tree.SwitchStateKey("defending", "waiting");
        tree.SwitchStateKey("moveToObjective", "onTheMove");
        targetUnit = null;
    }

    void Update()
    {
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
                UnitGeneral tmp = unitInRange.gameObject.GetComponent<UnitGeneral>();
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
                if (Vector3.Distance(unit.transform.position, targetUnit.transform.position) > unit.GetStats().attackRange + 0.5)
                {
                    tree.SwitchStateKey("combatAggressive", "noAdversary");
                    targetUnit = null;
                }
                break;

            // == AI state is defensive
            case "defend":
                //unit waits for an ennemy
                objective = unit.transform.position;
                if (targetUnit != null)
                {
                    tree.SwitchStateKey("defending", "fighting");
                    tree.SwitchStateKey("combatDefense", "adversaryFound");
                }
                break;

            case "backToDefense":
                //the unit decided to stop its fight
                unit.GoTo(objective);
                if(Vector3.Distance(unit.transform.position, objective) < 0.5)
                {
                    tree.SwitchStateKey("defending", "waiting");
                }
                break;

            case "attackDefense":
                //if not too far from its objective, unit attacks
                unit.AttackUnit(targetUnit);
                if(Vector3.Distance(unit.transform.position, objective) > 2.0)
                {
                    tree.SwitchStateKey("combatDefense", "noAdversary");
                    targetUnit = null;
                }
                break;

            // == AI state is followObjective
            case "move":
                //unit follows its pathfinding to the specified objective
                break;
            case "backToObjective":
                //the unit decided to stop its fight
                tree.SwitchStateKey("moveToObjective", "onTheMove");
                break;
            case "attackObjective":
                //unit attacks
                break;
            case "changeState":
                //unit arrived to its destination, and switches state
                tree.SwitchStateKey("state","defensive");
                break;

        }

        //Debug.Log(currentNode);
    }
}
