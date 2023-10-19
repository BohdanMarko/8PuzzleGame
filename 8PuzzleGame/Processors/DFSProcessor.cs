using _8PuzzleGame.Processors.Common;
using System.ComponentModel;
using System.Diagnostics;

namespace _8PuzzleGame.Processors;

public sealed class DFSProcessor : IProcessor
{
    private const byte STATE_LENGTH = 3;
    private const byte ZERO = 0;
    private const byte STEP = 1;

    private int GlobalNodesCount = 1;
    private int SkippedStatesCount = 0;

    private Stack<TreeNode> Stack = new Stack<TreeNode>();

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

            Console.Write("Write maximum tree depth >> ");
            int depth_limit = int.Parse(Console.ReadLine()!);
            Console.WriteLine();

            var stopwatch = Stopwatch.StartNew();

            Stack.Push(inputNode);

            while (Stack.Any())
            {
                TreeNode currentNode = Stack.Pop();
                if (CheckIfStateIsFinal(currentNode.State))
                {
                    PrintResult(currentNode);
                    stopwatch.Stop();
                    Console.WriteLine($"\nTotal time in ms: {stopwatch.ElapsedMilliseconds} ms");
                    Console.WriteLine($"Total time in s: {stopwatch.ElapsedMilliseconds / 1000} s");
                    return;
                }

                if (depth_limit == currentNode.Depth) continue;

                (sbyte I, sbyte J) init = GetZeroPosition(currentNode.State);

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

    private bool CompareArrays(byte[,] array1, byte[,] array2)
    {
        for (int i = 0; i < STATE_LENGTH; i++)
            for (int j = 0; j < STATE_LENGTH; j++)
                if (array1[i, j] != array2[i, j])
                    return false;

        return true;
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

    private bool CheckIfStateAlreadyExists(TreeNode node, byte[,] state)
    {
        if (node.IsRoot)
        {
            if (CompareArrays(node.State, state)) return true;
            else return false;
        }
        if (CompareArrays(node.State, state)) return true;
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
        Console.WriteLine(finalNode is not null ? "Final state found!" : "No final state found!");
        Console.WriteLine($"Total skipped states: {SkippedStatesCount}");
        Console.WriteLine($"Total nodes: {GlobalNodesCount}");
        if (finalNode is not null)
        {
            Console.WriteLine($"Final node depth: {finalNode.Depth}");
            PrintNode(finalNode.Parent, "Final state parent");
            PrintNode(finalNode, "Final state");
        }
    }

    private void PrintNode(TreeNode node, string description)
    {
        Console.WriteLine();
        Console.WriteLine($"{description} {node.NodeNumber}:");
        for (sbyte i = 0; i < STATE_LENGTH; i++)
        {
            for (sbyte j = 0; j < STATE_LENGTH; j++)
                Console.Write($"{node.State[i, j]} ");
            Console.WriteLine();
        }
    }
}
