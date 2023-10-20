using _8PuzzleGame.Processors.Common;
using System.Diagnostics;

namespace _8PuzzleGame.Processors;

public sealed class BFSProcessor : IProcessor
{
    private int GlobalNodesCount = 1;
    private int SkippedStatesCount = 0;

    private HashSet<int> VisitedStates = new HashSet<int>();
    private Queue<TreeNode> Queue = new Queue<TreeNode>();

    private readonly ProcessorHelper _helper;

    public BFSProcessor(ProcessorHelper helper)
    {
        _helper = helper;
    }

    public void Process()
    {
        try
        {
            var initialState = GetInputFromConsole();
            TreeNode inputNode = new(parent: null!, 1, initialState);

            var stopwatch = Stopwatch.StartNew();

            Queue.Enqueue(inputNode);
            VisitedStates.Add(inputNode.GetHashCode());

            while (Queue.Any())
            {
                TreeNode currentNode = Queue.Dequeue();
                if (_helper.CheckIfStateIsFinal(currentNode.State))
                {
                    PrintResult(currentNode);
                    stopwatch.Stop();
                    Console.WriteLine($"\nTotal time: {stopwatch.ElapsedMilliseconds} ms");
                    Console.WriteLine($"Total time in s: {stopwatch.ElapsedMilliseconds / 1000} s");
                    return;
                }

                (sbyte I, sbyte J) init = _helper.GetZeroPosition(currentNode.State);

                MoveZero(currentNode, init, MoveDirection.Down);     // ↓
                MoveZero(currentNode, init, MoveDirection.Left);     // ←
                MoveZero(currentNode, init, MoveDirection.Up);       // ↑
                MoveZero(currentNode, init, MoveDirection.Right);    // →
            }

            if (Queue.Any() is false) PrintResult(null!);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private void AddChild(TreeNode inputNode, byte[,] finalState)
    {
        TreeNode finalNode = new(inputNode, ++GlobalNodesCount, finalState);
        if (VisitedStates.Contains(finalNode.GetHashCode()))
        {
            SkippedStatesCount++;
            return;
        }
        inputNode.AddChild(finalNode);
        Queue.Enqueue(finalNode);
        VisitedStates.Add(finalNode.GetHashCode());
    }

    private void MoveZero(TreeNode inputNode, (sbyte I, sbyte J) init, MoveDirection moveDirection)
    {
        if (_helper.CheckIfMoveIsValid(init, moveDirection) is false) return;
        (sbyte X, sbyte Y) final = _helper.GetFinalZeroPosition(init, moveDirection);
        byte[,] finalState = _helper.SwapStateValues(inputNode.State, init, final);
        AddChild(inputNode, finalState);
    }

    private byte[,] GetInputFromConsole()
    {
        Console.WriteLine("Write initial state:");
        var line1 = Console.ReadLine()!.Split(" ").Select(x => Convert.ToByte(x)).ToArray();
        var line2 = Console.ReadLine()!.Split(" ").Select(x => Convert.ToByte(x)).ToArray();
        var line3 = Console.ReadLine()!.Split(" ").Select(x => Convert.ToByte(x)).ToArray();
        Console.WriteLine();
        return new byte[3, 3]
        {
            {line1[0], line1[1], line1[2]},
            {line2[0], line2[1], line2[2]},
            {line3[0], line3[1], line3[2]}
        };
    }

    private void PrintResult(TreeNode finalNode)
    {
        Console.WriteLine(finalNode is not null ? "Final state found!" : "No final state found!");
        Console.WriteLine($"Total visited states: {VisitedStates.Count}");
        Console.WriteLine($"Total skipped states: {SkippedStatesCount}");
        if (finalNode is not null)
        {
            Console.WriteLine($"Final node depth: {finalNode.Depth}");
            _helper.PrintNode(finalNode.Parent, "Final state parent");
            _helper.PrintNode(finalNode, "Final state");
        }
    }
}
