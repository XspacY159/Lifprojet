using System.Collections.Generic;
using UnityEngine;

namespace InputProvider.Graph
{
    [NodeTint("#4f008c")]
    [NodeWidth(150)]
    public class ChanceNode : BaseNode
    {
        [Input] public int entry;
        [Output] public int exit;

        public List<float> weights = new List<float>();

        public override NodeType GetNodeType()
        {
            return NodeType.chance;
        }
    }
}
