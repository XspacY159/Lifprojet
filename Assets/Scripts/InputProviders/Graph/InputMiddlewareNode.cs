using UnityEngine;
using System.Collections.Generic;

namespace InputProvider.Graph
{
    [NodeTint("#4287f5")]
    [NodeWidth(200)]
    public class InputMiddlewareNode : BaseNode
    {
        [Input] public int entry;
        [Output] public int exit;

        public string MWName;

        public List<string> stateKeys = new List<string>();
    }
}
