using turing_machines;

var c = new ConsoleTapeVisualizer();
var machineRunner = new MachineRunner(KnownMachines.SquareRootOf2);

var state = MachineRunner.MachineState.Empty;
while (true)
{
    var result = machineRunner.Move(state);
    if (result.IsT1)
    {
        Console.WriteLine(result.AsT1.Value);
        return -1;
    }

    if (!result.AsT0.ShouldContinue)
    {
        return 0;
    }

    state = result.AsT0;

    c.PrintTape(state.Tape);
}

class ConsoleTapeVisualizer
{
    public void PrintTape(Tape tape)
    {
        var cursorPosition = Console.GetCursorPosition();
        
        Console.WriteLine();
        Console.Write('|');

        var i = 0;
        foreach (var tapeElement in tape)
        {
            if (i == tape.CurrentIndex)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
            }
            Console.Write(tapeElement == KnownOperations.NullTerminator ? ' ' : tapeElement);
            if (i == tape.CurrentIndex)
            {
                Console.ResetColor();
            }
            Console.Write('|');

            i++;
        }
        
        Console.SetCursorPosition(0, cursorPosition.Top);
    }
}