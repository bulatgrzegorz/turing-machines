using System.Collections;
using Behaviour = System.Collections.Generic.IDictionary<Symbol, (Operation[] operations, char finalMConfig)>;
using static KnownOperations;

public static class KnownOperations
{
    public const char Schwa = 'e';
    public const char Ex = 'x';
    public const char One = '1';
    public const char Zero = '0';
    public const char NullTerminator = '\0';
    public static readonly Operation PutSchwa = new (OperationType.Put, Schwa);
    public static readonly Operation PutOne = new(OperationType.Put, One);
    public static readonly Operation PutZero = new (OperationType.Put, Zero);
    public static readonly Operation PutX = new (OperationType.Put, Ex);
    public static readonly Operation Right = new (OperationType.MoveRight);
    public static readonly Operation Left = new (OperationType.MoveLeft);
    public static readonly Operation Erase = new (OperationType.Erase);
}

public static class KnownMachines
{
    public static readonly Machine[] IncreasingRunsOfOnesSeparatedByZeros = {
        new('b', new Dictionary<Symbol, (Operation[], char)>()
        {
            { Symbol.None, (new[] { PutSchwa, Right, PutSchwa, Right, PutZero, Right, Right, PutZero, Left, Left }, 'o') },
        }),
        new('o', new Dictionary<Symbol, (Operation[], char)>()
        {
            { Symbol.One, (new[] { Right, PutX, Left, Left, Left }, 'o') },
            { Symbol.Zero, (Array.Empty<Operation>(), 'q') },
        }),
        new('q', new Dictionary<Symbol, (Operation[], char)>()
        {
            { Symbol.ZeroOrOne, (new[] { Right, Right }, 'q') },
            { Symbol.None, (new[] { PutOne, Left }, 'p') },
        }),
        new('p', new Dictionary<Symbol, (Operation[], char)>()
        {
            { Symbol.X, (new[] { Erase, Right }, 'q') },
            { Symbol.Schwa, (new[] { Right }, 'f') },
            { Symbol.None, (new[] { Left, Left }, 'p') },
        }),
        new('f', new Dictionary<Symbol, (Operation[], char)>()
        {
            { Symbol.Any, (new[] { Right, Right }, 'f') },
            { Symbol.None, (new[] { PutZero, Left, Left }, 'o') },
        }),
    };
}

public class MachineRunner
{
    private readonly ITapeVisualizer _tapeVisualizer;
    
    public MachineRunner(ITapeVisualizer tapeVisualizer)
    {
        _tapeVisualizer = tapeVisualizer;
    }

    //TODO: Starting symbol could be optional, we can find first one 
    public void Run(Machine[] machines, char startingSymbol)
    {
        var tape = new Tape();
        var symbol = startingSymbol;
        while (true)
        {
            var symbolMachine = machines.Single(x => x.MConfig == symbol);

            foreach (var (x, y) in symbolMachine.Behaviours)
            {
                if (!CompareSymbol(x, tape.Current)) continue;
                
                var shouldContinue = DoOperations(ref tape, y.operations);
                if (!shouldContinue)
                {
                    return;
                }

                symbol = y.finalMConfig;

                break;
            }
        }
    }

    private bool DoOperations(ref Tape tape, Operation[] operations)
    {
        foreach (var operation in operations)
        {
            switch (operation.Type)
            {
                case OperationType.Halt: return false;
                case OperationType.Erase: tape.SetValue('\0'); break;
                case OperationType.Put: tape.SetValue(operation.Symbol!.Value); break;
                case OperationType.MoveLeft: tape.MoveLeft(); break;
                case OperationType.MoveRight: tape.MoveRight(); break;
                default: throw new ArgumentOutOfRangeException(nameof(OperationType), operation.Type, "Given operation type is not supported.");
            }

            _tapeVisualizer.PrintTape(tape);
        }

        return true;
    }

    private static bool CompareSymbol(Symbol symbol, char value)
    {
        return symbol switch
        {
            Symbol.One => value == One,
            Symbol.Zero => value == Zero,
            Symbol.ZeroOrOne => value is Zero or One,
            Symbol.X => value == Ex,
            Symbol.Schwa => value == Schwa,
            Symbol.None => value == NullTerminator,
            Symbol.Any => value != NullTerminator,
            _ => throw new ArgumentOutOfRangeException(nameof(symbol), symbol, null)
        };
    }
}



public enum Symbol
{
    One,
    Zero,
    ZeroOrOne,
    Any,
    None,
    X,
    Schwa
}

public enum OperationType
{
    MoveRight,
    MoveLeft,
    Put,
    Erase,
    Halt
}

public struct Tape : IEnumerable<char>
{
    private const int DefaultSize = 2;//1024;
    
    public Tape()
    {
        InternalTape = new char[DefaultSize];
        CurrentIndex = 0;
        Capacity = 0;
    }
    private char[] InternalTape { get; set; }
    public int CurrentIndex { get; private set; }
    private int Capacity { get; set; }
    public char Current => InternalTape[CurrentIndex];
    public void SetValue(char value) => InternalTape[CurrentIndex] = value;
    public void MoveLeft() => CurrentIndex -= 1;
    public void MoveRight()
    {
        CurrentIndex += 1;
        Capacity = Capacity > CurrentIndex ? Capacity : CurrentIndex;
        
        if (CurrentIndex >= InternalTape.Length)
        {
            var tape = InternalTape;
            Array.Resize(ref tape, InternalTape.Length * 2);

            InternalTape = tape;
        }
    }

    public void Print()
    {
        Console.WriteLine();
        Console.Write('|');
        for (var i = 0; i < Capacity + 1; i++)
        {
            if (i == CurrentIndex)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
            }
            Console.Write(InternalTape[i]);
            if (i == CurrentIndex)
            {
                Console.ResetColor();
            }
            Console.Write('|');
        }
    }

    public IEnumerator<char> GetEnumerator()
    {
        for (var i = 0; i < Capacity + 1; i++)
        {
            yield return InternalTape[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public interface ITapeVisualizer
{
    void PrintTape(Tape tape);
}
public record Operation(OperationType Type, char? Symbol = null);
public record Machine(char MConfig, IDictionary<Symbol, (Operation[] operations, char finalMConfig)> Behaviours);