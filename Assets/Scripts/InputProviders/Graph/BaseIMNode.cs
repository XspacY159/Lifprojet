using XNode;

namespace InputProvider.Graph
{
    public enum NodeType
    {
        InputMiddleware,
        wait,
        chance,
        start
    };

    public class BaseNode : Node
    {
        public virtual NodeType GetNodeType() { return 0; }

        public virtual float GetWaitTime() { return 0; }
    }
}
