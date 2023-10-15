using System.ComponentModel;
using System.Diagnostics;

namespace BFS;

public sealed class TreeNodeProcessor
{
    enum MoveDirection
    {
        Up,
        Down,
        Right,
        Left
    }

    private const byte STATE_LENGTH = 3;
    private const byte ZERO = 0;
    private const byte STEP = 1;

    private int GlobalNodesCount = 1;
    private int SkippedStatesCount = 0;

    private HashSet<int> VisitedStates = new HashSet<int>();
    private Queue<TreeNode> Queue = new Queue<TreeNode>();

    private readonly byte[,] FinalState = new byte[STATE_LENGTH, STATE_LENGTH]
    {
        {1, 2, 3},
        {4, 5, 6},
        {7, 8, ZERO}
    };

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
                if (CheckIfStateIsFinal(currentNode.State))
                {
                    PrintResult(currentNode);
                    stopwatch.Stop();
                    Console.WriteLine($"Total time: {stopwatch.ElapsedMilliseconds} ms");
                    return;
                }

                (sbyte I, sbyte J) init = GetZeroPosition(currentNode.State);

                MoveZero(currentNode, init, MoveDirection.Down);     // ↓
                MoveZero(currentNode, init, MoveDirection.Left);     // ←
                MoveZero(currentNode, init, MoveDirection.Up);       // ↑
                MoveZero(currentNode, init, MoveDirection.Right);    // →
            }

            if (Queue.Any() is false)
                Console.WriteLine("No final state found!");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private (sbyte I, sbyte J) GetZeroPosition(byte[,] state)
    {
        for (sbyte i = 0; i < STATE_LENGTH; i++)
            for (sbyte j = 0; j < STATE_LENGTH; j++)
                if (state[i, j] == ZERO)
                    return (i, j);

        throw new Exception("Zero is missing in current state!");
    }

    private (sbyte I, sbyte J) GetFinalZeroPosition((sbyte I, sbyte J) init, MoveDirection moveDirection) => moveDirection switch
    {
        MoveDirection.Up => (Convert.ToSByte(init.I - STEP), init.J),
        MoveDirection.Down => (Convert.ToSByte(init.I + STEP), init.J),
        MoveDirection.Right => (init.I, Convert.ToSByte(init.J + STEP)),
        MoveDirection.Left => (init.I, Convert.ToSByte(init.J - STEP)),
        _ => throw new InvalidEnumArgumentException("Invalid Move Type!")
    };

    private byte[,] SwapStateValues(byte[,] inputState, (sbyte I, sbyte J) init, (sbyte I, sbyte J) final)
    {
        byte[,] finalState = (inputState.Clone() as byte[,])!;
        finalState[init.I, init.J] = finalState[final.I, final.J];
        finalState[final.I, final.J] = ZERO;
        return finalState;
    }

    private bool CheckIfStateIsFinal(byte[,] state)
    {
        for (sbyte i = 0; i < STATE_LENGTH; i++)
            for (sbyte j = 0; j < STATE_LENGTH; j++)
                if (state[i, j] != FinalState[i, j])
                    return false;

        return true;
    }

    private bool CheckIfMoveIsValid((sbyte I, sbyte J) init, MoveDirection moveDirection) => moveDirection switch
    {
        MoveDirection.Up => init.I > ZERO,
        MoveDirection.Down => init.I < (STATE_LENGTH - STEP),
        MoveDirection.Right => init.J < (STATE_LENGTH - STEP),
        MoveDirection.Left => init.J > ZERO,
        _ => throw new InvalidEnumArgumentException("Invalid Move Type!")
    };

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
        if (CheckIfMoveIsValid(init, moveDirection) is false) return;
        (sbyte X, sbyte Y) final = GetFinalZeroPosition(init, moveDirection);
        byte[,] finalState = SwapStateValues(inputNode.State, init, final);
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
        Console.WriteLine("Final state found!");
        Console.WriteLine($"Total visited states: {VisitedStates.Count}");
        Console.WriteLine($"Total skipped states: {SkippedStatesCount}");
        Console.WriteLine($"Final node depth: {finalNode.Depth}");
        Console.WriteLine();
        PrintNode(finalNode.Parent, "Final state parent");
        PrintNode(finalNode, "Final state");
    }

    private void PrintNode(TreeNode node, string description)
    {
        Console.WriteLine($"{description} {node.NodeNumber}:");
        for (sbyte i = 0; i < STATE_LENGTH; i++)
        {
            for (sbyte j = 0; j < STATE_LENGTH; j++)
                Console.Write($"{node.State[i, j]} ");
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}