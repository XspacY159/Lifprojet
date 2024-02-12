using UnityEngine;

namespace InputProvider.Graph
{
    [NodeTint("#d1133c")]
    [NodeWidth(150)]
    public class WaitNode : BaseNode
    {
        [Input] public int entry;
        [Output] public int exit;

        [SerializeField] private float waitTime;

        public override float GetWaitTime()
        {
            return waitTime;
        }

        public override NodeType GetNodeType()
        {
            return NodeType.wait;
        }
    }
}
