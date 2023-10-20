using _8PuzzleGame.Processors;
using _8PuzzleGame.Processors.Common;

IProcessor processor = CreateProcessor(ProcessorType.DFS);
processor.Process();

IProcessor CreateProcessor(ProcessorType type)
{
    ProcessorHelper helper = new();
    return type switch
    {
        ProcessorType.BFS => new BFSProcessor(helper),
        ProcessorType.DFS => new DFSProcessor(helper),
        _ => throw new NotImplementedException(),
    };
};