using UnityEngine;
using System;
using InputProvider.Graph;

public class EnnemyBTSolver : MonoBehaviour
{
    [SerializeField]
    private IPTreeReader tree;

    [SerializeField]
    private UnitGeneral unit;
    [SerializeField]
    private AITeamController team;

    private UnitMessages message;

    private UnitGeneral targetUnit;
    [SerializeField]
    private POI targetObjective;
    private Vector3 targetPosition;

    private bool treeEnabled;


    void Start()
    {
        reset();
        treeEnabled = true;
    }

    void Update()
    {
        if (UnitSelectionController.Instance == null) return;
        if (treeEnabled)
        {
            switch (unit.baseState)
            {
                case AIState.Aggressive:
                    tree.SwitchStateKey("state", "aggressive");
                    break;
                case AIState.Defensive:
                    unit.currentActionPriority = 2;
                    tree.SwitchStateKey("state", "defensive");
                    break;
                case AIState.FollowObjective:
                    tree.SwitchStateKey("state", "followObjective");
                    break;
            }
            solve();
        }
    }

    public void reset()
    {
        tree.SwitchStateKey("combatAggressive", "noAdversary");
        tree.SwitchStateKey("defending", "waiting");
        tree.SwitchStateKey("moveToObjective", "onTheMove");
        targetUnit = null;
    }

    public void solve()
    {
        string currentNode = tree.ResolveGraph();

        if(targetUnit == null)
        {
            foreach (Collider unitInRange in unit.UnitsInRange())
            {
                if (unitInRange.gameObject == unit.gameObject) continue;
                UnitGeneral tmp = UnitManager.Instance.GetUnit(unitInRange.gameObject);
                if (tmp.GetTeam() != unit.GetTeam())
                {
                    targetUnit = tmp;
                    break;
                }
                else
                {
                    team.SendMessageToUnit(new UnitMessages(unit.GetUnitID(), MessageObject.AskGroup), tmp.GetUnitID());
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
                    team.SendMessageToAll(new UnitMessages(unit.GetUnitID(), MessageObject.AttackTarget, unit.transform.position, _targetUnit: targetUnit));
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
                unit.baseState = AIState.Defensive;
                break;

        }

        message = team.ReadMessage(unit.GetUnitID());
        if (message != null)
        {
            if (message.priority >= unit.currentActionPriority)
            {
                switch (message.messageObject)
                {
                    case MessageObject.AttackTarget:
                        reset();
                        targetUnit = message.targetUnit;
                        tree.SwitchStateKey("state", "aggressive");
                        tree.SwitchStateKey("combatAggressive", "adversaryFound");
                        break;

                    case MessageObject.DefendPosition:
                        reset();
                        targetPosition = message.position;
                        tree.SwitchStateKey("state", "defensive");
                        tree.SwitchStateKey("combatDefense", "noAdversary");
                        break;

                    case MessageObject.GoToObjective:
                        reset();
                        targetObjective = message.targetObjective;
                        if (targetObjective != null)
                            tree.SwitchStateKey("state", "followObjective");
                        break;

                    case MessageObject.AskGroup:
                        if (message.emitter.CompareTo(unit.GetUnitID()) > 0)
                        {
                            Guid newGroup = team.CreateGroup();
                            team.JoinGroup(unit, newGroup);
                            team.SendMessageToUnit(
                                new UnitMessages(unit.GetUnitID(), MessageObject.JoinGroup, newGroup, 5),
                                message.emitter);
                        }
                        break;

                    case MessageObject.JoinGroup:
                        reset();
                        treeEnabled = false;
                        team.JoinGroup(unit, message.groupID);
                        break;
                }
            }
            message = null;
        }
    }
}
