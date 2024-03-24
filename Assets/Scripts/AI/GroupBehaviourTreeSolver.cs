using UnityEngine;

using InputProvider.Graph;

public class GroupBehaviourTreeSolver : MonoBehaviour
{
    private IPTreeReader tree;

    private AITeamController team;

    private UnitMessages message;

    private UnitGeneral targetUnit;
    private POI targetObjective;
    private Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        reset();
    }

    public GroupBehaviourTreeSolver(IPTreeReader _tree, AITeamController _team)
    {
        tree = _tree;
        team = _team;

        reset();
    }

    // Update is called once per frame
    void Update()
    {
        solve();
    }

    public void reset()
    {
        tree.SwitchStateKey("combatAggressive", "noAdversary");
        tree.SwitchStateKey("defending", "waiting");
        tree.SwitchStateKey("moveToObjective", "onTheMove");
        //targetUnit = null;
    }

    public void solve()
    {
        string currentNode = tree.ResolveGraph();

        
    }
}

