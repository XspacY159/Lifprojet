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
    private POI objective;

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
                    unit.AttackUnit(targetUnit);
                }
                break;
            case "attackAggressive":
                //unit attacks
                break;

            // == AI state is defensive
            case "Defend":
                //unit waits for an ennemy
                break;
            case "BackToDefense":
                //the unit decided to stop its fight
                tree.SwitchStateKey("defending", "Waiting");
                break;
            case "attackDefense":
                //unit attacks
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
    }
}
