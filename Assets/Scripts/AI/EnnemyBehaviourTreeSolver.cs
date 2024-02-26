using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using InputProvider.Graph;

public class EnnemyBTSolver : MonoBehaviour
{
    [SerializeField]
    private IPTreeReader tree;

    public void solve()
    {
        string currentNode = tree.ResolveGraph();

        switch (currentNode)
        {
            // == AI state is aggressive
            case "idleAggressive":
                //unit waits for an ennemy
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
