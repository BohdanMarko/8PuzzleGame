namespace BFS;

public sealed class TreeNode
{
    public TreeNode Parent { get; set; }
    public List<TreeNode> Children { get; set; } = new();
    public byte[,] State { get; set; }
    public int Depth => GetDepth(this);
    public int NodeNumber { get; set; }

    public TreeNode(TreeNode parent, int nodeNumber, byte[,] state)
    {
        Parent = parent;
        NodeNumber = nodeNumber;
        State = state;
    }

    public void AddChild(TreeNode child) => Children.Add(child);

    private int GetDepth(TreeNode current)
    {
        if (current.Parent is null) return 0;
        return 1 + GetDepth(current.Parent);
    }

    public override int GetHashCode()
    {
        byte[] stateArray = new byte[9];
        Buffer.BlockCopy(State, 0, stateArray, 0, 9);
        int result = int.Parse(string.Join("", stateArray));
        return result;
    }
}
