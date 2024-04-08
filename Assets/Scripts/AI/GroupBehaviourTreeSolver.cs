using UnityEngine;

using InputProvider.Graph;

public class GroupBehaviourTreeSolver : MonoBehaviour
{
    [SerializeField] private IPTreeReader tree;

    private AITeamController team;
    private GroupController group;

    private UnitMessages message;

    private UnitGeneral targetUnit;
    private POI targetObjective;
    private Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        ResetState();
        Debug.Log("tree start");
    }

    public void Setup(AITeamController _team, GroupController _group)
    {
        team = _team;
        group = _group;

        ResetState();
    }

    // Update is called once per frame
    void Update()
    {
        solve();
    }

    public void ResetState()
    {
        tree.SwitchStateKey("combatAggressive", "noAdversary");
        tree.SwitchStateKey("defending", "waiting");
        tree.SwitchStateKey("moveToObjective", "onTheMove");
        targetUnit = null;
    }

    public void solve()
    {
        string currentNode = tree.ResolveGraph();

        if (targetUnit == null)
        {
            targetUnit = group.FindAdversary();
        }

        switch (currentNode)
        {
            // == AI state is aggressive
            case "idleAggressive":
                //unit waits for an ennemy
                group.Roam(7, group.GetGroupPosition());
                if (targetUnit != null)
                {
                    tree.SwitchStateKey("combatAggressive", "adversaryFound");
                    //team.SendMessageToAll(new UnitMessages(group.GetMessageAddress(), MessageObject.AttackTarget, unit.transform.position, _targetUnit: targetUnit));
                    //could be wise to make a special function in group to build correct messages
                }
                break;

            case "attackAggressive":
                //unit attacks
                group.AttackUnit(targetUnit);
                if (targetUnit == null || Vector3.Distance(group.GetGroupPosition(), targetUnit.transform.position) > 5 + 0.7)
                {
                    tree.SwitchStateKey("combatAggressive", "noAdversary");
                    targetUnit = null;
                }
                break;

            case "defend":
                targetPosition = group.GetGroupPosition();
                group.Roam(2, group.GetGroupPosition());
                if (targetUnit != null)
                {
                    tree.SwitchStateKey("defending", "fighting");
                    tree.SwitchStateKey("combatDefense", "adversaryFound");
                }
                break;

            case "backToDefense":
                //the unit decided to stop its fight
                group.GoTo(targetPosition);
                if (Vector3.Distance(group.GetGroupPosition(), targetPosition) < 0.5)
                {
                    tree.SwitchStateKey("defending", "waiting");
                }
                break;

            case "attackDefense":
                //if not too far from its objective, unit attacks
                group.AttackUnit(targetUnit);
                if (Vector3.Distance(group.GetGroupPosition(), targetPosition) > 2.0)
                {
                    tree.SwitchStateKey("combatDefense", "noAdversary");
                    targetUnit = null;
                }
                break;


            // == AI state is followObjective
            case "move":
                //unit follows its pathfinding to the specified objective
                targetPosition = targetObjective.transform.position;
                group.GoTo(targetPosition);
                group.TryInteract(group.GetOneUnit(),targetObjective);
                if (Vector3.Distance(group.GetGroupPosition(), targetPosition) < 0.5)
                {
                    tree.SwitchStateKey("moveToObjective", "arrived");
                    break;
                }
                if (targetUnit != null)
                {
                    targetPosition = group.GetGroupPosition();
                    tree.SwitchStateKey("moveToObjective", "fighting");
                    tree.SwitchStateKey("combatObjective", "adversaryFound");
                }
                break;
            case "backToObjective":
                //the unit decided to stop its fight
                group.GoTo(targetPosition);
                if (Vector3.Distance(group.GetGroupPosition(), targetPosition) < 0.5)
                {
                    tree.SwitchStateKey("moveToObjective", "onTheMove");
                }
                break;

            case "attackObjective":
                //if not too far from its objective, unit attacks
                group.AttackUnit(targetUnit);
                if (Vector3.Distance(group.GetGroupPosition(), targetPosition) > 2.0)
                {
                    tree.SwitchStateKey("combatObjective", "noAdversary");
                    targetUnit = null;
                }
                break;
            case "changeState":
                //unit arrived to its destination, and switches state
                tree.SwitchStateKey("state", "defensive");
                break;
        }

        message = group.ReadMessages();
        if (message != null)
        {
            switch (message.messageObject)
            {
                case MessageObject.AskGroup:
                    Debug.Log("asking to join group");
                    team.SendMessageToUnit(
                        new UnitMessages(group.GetMessageAddress(), MessageObject.JoinGroup, group.GetID(), 5),
                        message.emitter);
                    break;
            }
        }
    }
}

