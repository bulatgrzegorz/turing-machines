var machineRunner = new MachineRunner(new ConsoleTapeVisualizer());
machineRunner.Run(KnownMachines.IncreasingRunsOfOnesSeparatedByZeros, 'b');

class ConsoleTapeVisualizer : ITapeVisualizer
{
    public void PrintTape(Tape tape)
    {
        Thread.Sleep(500);
        
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