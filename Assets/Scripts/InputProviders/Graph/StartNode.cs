namespace InputProvider.Graph
{
    [NodeTint("#130069")]
    [NodeWidth(60)]
    public class StartNode : BaseNode
    {
        [Output] public int exit;

        public override NodeType GetNodeType()
        {
            return NodeType.start;
        }
    }
}