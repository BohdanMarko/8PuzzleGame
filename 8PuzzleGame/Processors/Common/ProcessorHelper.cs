using System.ComponentModel;

namespace _8PuzzleGame.Processors.Common;

public sealed class ProcessorHelper
{
    private const byte STATE_LENGTH = 3;
    private const byte ZERO = 0;
    private const byte STEP = 1;

    private readonly byte[,] FinalState = new byte[STATE_LENGTH, STATE_LENGTH]
    {
            {1, 2, 3},
            {4, 5, 6},
            {7, 8, ZERO}
    };

    public (sbyte I, sbyte J) GetZeroPosition(byte[,] state)
    {
        for (sbyte i = 0; i < STATE_LENGTH; i++)
            for (sbyte j = 0; j < STATE_LENGTH; j++)
                if (state[i, j] == ZERO)
                    return (i, j);

        throw new Exception("Zero is missing in current state!");
    }

    public (sbyte I, sbyte J) GetFinalZeroPosition((sbyte I, sbyte J) init, MoveDirection moveDirection) => moveDirection switch
    {
        MoveDirection.Up => (Convert.ToSByte(init.I - STEP), init.J),
        MoveDirection.Down => (Convert.ToSByte(init.I + STEP), init.J),
        MoveDirection.Right => (init.I, Convert.ToSByte(init.J + STEP)),
        MoveDirection.Left => (init.I, Convert.ToSByte(init.J - STEP)),
        _ => throw new InvalidEnumArgumentException("Invalid Move Type!")
    };

    public byte[,] SwapStateValues(byte[,] inputState, (sbyte I, sbyte J) init, (sbyte I, sbyte J) final)
    {
        byte[,] finalState = (inputState.Clone() as byte[,])!;
        finalState[init.I, init.J] = finalState[final.I, final.J];
        finalState[final.I, final.J] = ZERO;
        return finalState;
    }

    public bool CompareArrays(byte[,] array1, byte[,] array2)
    {
        for (int i = 0; i < STATE_LENGTH; i++)
            for (int j = 0; j < STATE_LENGTH; j++)
                if (array1[i, j] != array2[i, j])
                    return false;

        return true;
    }

    public bool CheckIfMoveIsValid((sbyte I, sbyte J) init, MoveDirection moveDirection) => moveDirection switch
    {
        MoveDirection.Up => init.I > ZERO,
        MoveDirection.Down => init.I < (STATE_LENGTH - STEP),
        MoveDirection.Right => init.J < (STATE_LENGTH - STEP),
        MoveDirection.Left => init.J > ZERO,
        _ => throw new InvalidEnumArgumentException("Invalid Move Type!")
    };

    public bool CheckIfStateIsFinal(byte[,] state)
    {
        for (sbyte i = 0; i < STATE_LENGTH; i++)
            for (sbyte j = 0; j < STATE_LENGTH; j++)
                if (state[i, j] != FinalState[i, j])
                    return false;

        return true;
    }

    public void PrintNode(TreeNode node, string description)
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
