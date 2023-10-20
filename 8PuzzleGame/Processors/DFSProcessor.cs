using _8PuzzleGame.Processors.Common;
using System.Diagnostics;

namespace _8PuzzleGame.Processors;

public sealed class DFSProcessor : IProcessor
{
    private int GlobalNodesCount = 1;
    private int SkippedStatesCount = 0;

    private Stack<TreeNode> Stack = new Stack<TreeNode>();

    private readonly ProcessorHelper _helper;

    public DFSProcessor(ProcessorHelper helper)
    {
        _helper = helper;
    }

    public void Process()
    {
        try
        {
            (byte[,] initialState, int depthLimit) = GetInputFromConsole();
            TreeNode inputNode = new(parent: null!, 1, initialState);

            var stopwatch = Stopwatch.StartNew();

            Stack.Push(inputNode);

            while (Stack.Any())
            {
                TreeNode currentNode = Stack.Pop();
                if (_helper.CheckIfStateIsFinal(currentNode.State))
                {
                    stopwatch.Stop();
                    PrintResult(currentNode);
                    Console.WriteLine($"\nTotal time in ms: {stopwatch.ElapsedMilliseconds} ms");
                    Console.WriteLine($"Total time in s: {stopwatch.ElapsedMilliseconds / 1000} s");
                    return;
                }

                if (depthLimit == currentNode.Depth) continue;

                (sbyte I, sbyte J) init = _helper.GetZeroPosition(currentNode.State);

                MoveZero(currentNode, init, MoveDirection.Up);       // ↑
                MoveZero(currentNode, init, MoveDirection.Right);    // →
                MoveZero(currentNode, init, MoveDirection.Left);     // ←
                MoveZero(currentNode, init, MoveDirection.Down);     // ↓
            }

            if (Stack.Any() is false) PrintResult(null!);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private bool CheckIfStateAlreadyExists(TreeNode node, byte[,] state)
    {
        if (node.IsRoot)
        {
            if (_helper.CompareArrays(node.State, state)) return true;
            else return false;
        }
        if (_helper.CompareArrays(node.State, state)) return true;
        else return CheckIfStateAlreadyExists(node.Parent, state);
    }

    private void AddChild(TreeNode inputNode, byte[,] finalState)
    {
        TreeNode finalNode = new(inputNode, ++GlobalNodesCount, finalState);
        if (CheckIfStateAlreadyExists(finalNode.Parent, finalState))
        {
            SkippedStatesCount++;
            return;
        }
        inputNode.AddChild(finalNode);
        Stack.Push(finalNode);
    }

    private void MoveZero(TreeNode inputNode, (sbyte I, sbyte J) init, MoveDirection moveDirection)
    {
        if (_helper.CheckIfMoveIsValid(init, moveDirection) is false) return;
        (sbyte X, sbyte Y) final = _helper.GetFinalZeroPosition(init, moveDirection);
        byte[,] finalState = _helper.SwapStateValues(inputNode.State, init, final);
        AddChild(inputNode, finalState);
    }

    private (byte[,], int) GetInputFromConsole()
    {
        Console.WriteLine("Write initial state:");
        var line1 = Console.ReadLine()!.Split(" ").Select(x => Convert.ToByte(x)).ToArray();
        var line2 = Console.ReadLine()!.Split(" ").Select(x => Convert.ToByte(x)).ToArray();
        var line3 = Console.ReadLine()!.Split(" ").Select(x => Convert.ToByte(x)).ToArray();
        Console.WriteLine();
        Console.Write("Write maximum tree depth >> ");
        int depth_limit = int.Parse(Console.ReadLine()!);
        Console.WriteLine();
        return (new byte[3, 3]
        {
            {line1[0], line1[1], line1[2]},
            {line2[0], line2[1], line2[2]},
            {line3[0], line3[1], line3[2]}
        }, 
        depth_limit);
    }

    private void PrintResult(TreeNode finalNode)
    {
        Console.WriteLine(finalNode is not null ? "Final state found!" : "No final state found!");
        Console.WriteLine($"Total skipped states: {SkippedStatesCount}");
        Console.WriteLine($"Total nodes: {GlobalNodesCount}");
        if (finalNode is not null)
        {
            Console.WriteLine($"Final node depth: {finalNode.Depth}");
            _helper.PrintNode(finalNode.Parent, "Final state parent");
            _helper.PrintNode(finalNode, "Final state");
        }
    }
}
