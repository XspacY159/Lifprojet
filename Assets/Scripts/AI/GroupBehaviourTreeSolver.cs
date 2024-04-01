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
                //Roam(7, transform.position);
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

