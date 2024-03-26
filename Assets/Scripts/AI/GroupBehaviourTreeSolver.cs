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
            //foreach (unit in )
        }
    }
}

