using UnityEngine;
using System;
using InputProvider.Graph;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine.InputSystem;

public class EnnemyBTSolver : MonoBehaviour
{
    [Header("Unit Components")]
    [SerializeField] private UnitGeneral unit;
    [SerializeField] private AITeamController team;
    [SerializeField] private IPTreeReader tree;
    private bool treeEnabled;

    [Header("Objectives")]
    [SerializeField] private POI targetObjective;
    //On pourra rajouter des potentiels POI ici (une liste de POI?)

    private UnitMessages message;
    private UnitGeneral targetUnit;
    private Vector3 targetPosition;
    private Vector3 defensiveRoamPoint;
    private AIState currentBaseState;

    private void OnEnable()
    {
        tree.onStateKeyChanged += OnStateKeyChanged;
        defensiveRoamPoint = transform.position;
    }

    private void OnDisable()
    {
        tree.onStateKeyChanged -= OnStateKeyChanged;
    }

    void Start()
    {
        ResetState();
        treeEnabled = true;
    }

    void Update()
    {
        if (UnitSelectionController.Instance == null) return;
        if (!treeEnabled) return;

        if (currentBaseState != unit.baseState)
        {
            currentBaseState = unit.baseState;
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

        }
        Solve();
    }

    public void ResetState()
    {
        tree.SwitchStateKey("combatAggressive", "noAdversary");
        tree.SwitchStateKey("defending", "waiting");
        tree.SwitchStateKey("moveToObjective", "onTheMove");
        targetUnit = null;
    }

    public void Solve()
    {
        string currentNode = tree.ResolveGraph();

        if(targetUnit == null)
        {
            foreach (Collider unitInRange in unit.UnitsInRange())
            {
                if (unitInRange.gameObject == unit.gameObject) continue;
                UnitGeneral tmp = UnitManager.Instance.GetUnit(unitInRange.gameObject);
                if(tmp == null) continue;   //if absent, units that are in range of each other throw an exception on start
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
                Roam(7, transform.position);
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
                Roam(2, defensiveRoamPoint);
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
                unit.TryInteract(targetObjective);
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

        //Messages
        message = team.ReadMessage(unit.GetUnitID());
        if (message != null)
        {
            if (message.priority >= unit.currentActionPriority)
            {
                switch (message.messageObject)
                {
                    case MessageObject.AttackTarget:
                        ResetState();
                        targetUnit = message.targetUnit;
                        tree.SwitchStateKey("state", "aggressive");
                        tree.SwitchStateKey("combatAggressive", "adversaryFound");
                        break;

                    case MessageObject.DefendPosition:
                        ResetState();
                        targetPosition = message.position;
                        tree.SwitchStateKey("state", "defensive");
                        tree.SwitchStateKey("combatDefense", "noAdversary");
                        break;

                    case MessageObject.GoToObjective:
                        ResetState();
                        targetObjective = message.targetObjective;
                        if (targetObjective != null)
                            tree.SwitchStateKey("state", "followObjective");
                        break;

                    case MessageObject.AskGroup:
                        Debug.Log("asking for group creation");
                        if (message.emitter.CompareTo(unit.GetUnitID()) > 0)
                        {
                            ResetState();
                            treeEnabled = false;
                            Guid newGroup = team.CreateGroup();
                            team.JoinGroup(unit, newGroup);
                            team.SendMessageToUnit(
                                new UnitMessages(unit.GetUnitID(), MessageObject.JoinGroup, newGroup, 5),
                                message.emitter);
                        }
                        break;

                    case MessageObject.JoinGroup:
                        ResetState();
                        treeEnabled = false;
                        team.JoinGroup(unit, message.groupID);
                        Debug.Log("joined a group");
                        break;
                }
            }
            message = null;
        }
    }

    private void Roam(float maxDistance, Vector3 fixedPoint)    //move to unit controller
    {
        if (!TimerManager.StartTimer(3, "RandRoam" + unit.GetUnitID()))
        {
            float randX = Mathf.Clamp(fixedPoint.x + UnityEngine.Random.Range(-maxDistance, maxDistance), 
                0, TerrainManager.Instance.GetTerrainSize().x - 1);
            float randZ = Mathf.Clamp(fixedPoint.z + UnityEngine.Random.Range(-maxDistance, maxDistance),
                0, TerrainManager.Instance.GetTerrainSize().y - 1);
            Vector3 randPos = new Vector3(randX, 0, randZ);
            unit.GoTo(randPos);
        }
    }

    private void OnStateKeyChanged(string key)
    {
        if (key == "defend") defensiveRoamPoint = transform.position;
        TimerManager.Cancel("RandRoam" + unit.GetUnitID());
    }
}
