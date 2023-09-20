using System.Collections;
using OneOf;
using OneOf.Types;

namespace turing_machines;

public static class KnownOperations
{
    public const char Schwa = 'e';
    public const char Ex = 'x';
    public const char Y = 'y';
    public const char Zet = 'z';
    public const char R = 'r';
    public const char S = 's';
    public const char U = 'u';
    public const char V = 'v';
    public const char T = 't';
    public const char W= 'w';
    public const char One = '1';
    public const char Zero = '0';
    public const char NullTerminator = '\0';
    public static readonly Operation PutSchwa = new (OperationType.Put, Schwa);
    public static readonly Operation PutOne = new(OperationType.Put, One);
    public static readonly Operation PutZero = new (OperationType.Put, Zero);
    public static readonly Operation PutX = new (OperationType.Put, Ex);
    public static readonly Operation PutS = new (OperationType.Put, S);
    public static readonly Operation PutZ = new (OperationType.Put, Zet);
    public static readonly Operation PutR = new (OperationType.Put, R);
    public static readonly Operation PutV = new (OperationType.Put, V);
    public static readonly Operation PutU = new (OperationType.Put, U);
    public static readonly Operation PutY = new (OperationType.Put, Y);
    public static readonly Operation PutT = new (OperationType.Put, T);
    public static readonly Operation PutW = new (OperationType.Put, W);
    public static readonly Operation Right = new (OperationType.MoveRight);
    public static readonly Operation Left = new (OperationType.MoveLeft);
    public static readonly Operation Erase = new (OperationType.Erase);
}

public static class KnownMachines
{
    public static readonly Machine[] IncreasingRunsOfOnesSeparatedByZeros = {
        new("b", new Dictionary<Symbol, (Operation[], string)>()
        {
            { Symbol.None, (new[] { KnownOperations.PutSchwa, KnownOperations.Right, KnownOperations.PutSchwa, KnownOperations.Right, KnownOperations.PutZero, KnownOperations.Right, KnownOperations.Right, KnownOperations.PutZero, KnownOperations.Left, KnownOperations.Left }, "o") },
        }),
        new("o", new Dictionary<Symbol, (Operation[], string)>()
        {
            { Symbol.One, (new[] { KnownOperations.Right, KnownOperations.PutX, KnownOperations.Left, KnownOperations.Left, KnownOperations.Left }, "o") },
            { Symbol.Zero, (Array.Empty<Operation>(), "q") },
        }),
        new("q", new Dictionary<Symbol, (Operation[], string)>()
        {
            { Symbol.ZeroOrOne, (new[] { KnownOperations.Right, KnownOperations.Right }, "q") },
            { Symbol.None, (new[] { KnownOperations.PutOne, KnownOperations.Left }, "p") },
        }),
        new("p", new Dictionary<Symbol, (Operation[], string)>()
        {
            { Symbol.X, (new[] { KnownOperations.Erase, KnownOperations.Right }, "q") },
            { Symbol.Schwa, (new[] { KnownOperations.Right }, "f") },
            { Symbol.None, (new[] { KnownOperations.Left, KnownOperations.Left }, "p") },
        }),
        new("f", new Dictionary<Symbol, (Operation[], string)>()
        {
            { Symbol.Any, (new[] { KnownOperations.Right, KnownOperations.Right }, "f") },
            { Symbol.None, (new[] { KnownOperations.PutZero, KnownOperations.Left, KnownOperations.Left }, "o") },
        }),
    };

    public static readonly Machine[] IncreasingNumberInPlace =
    {
        new("b", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.None, (new[] { KnownOperations.PutZero }, "i") }
        }),
        new("i", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.Zero, (new[] { KnownOperations.PutOne }, "r") },
            { Symbol.One, (new[] { KnownOperations.PutZero, KnownOperations.Left }, "i") },
            { Symbol.None, (new[] { KnownOperations.PutOne }, "r") },
        }),
        new("r", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.None, (new[] { KnownOperations.Left }, "i") },
            { Symbol.Any, (new[] { KnownOperations.Right }, "r") },
        }),
    };
    
    public static readonly Machine[] SquareRootOf2 =
    {
        new("begin", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.None, (new []{KnownOperations.PutSchwa, KnownOperations.Right, KnownOperations.PutOne}, "new")},
        }),
        new("new", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.Schwa, (new []{KnownOperations.Right}, "mark-digits")},
            { Symbol.Any, (new []{KnownOperations.Left}, "new")},
        }),
        new("mark-digits", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.Zero, (new []{KnownOperations.Right, KnownOperations.PutX, KnownOperations.Right}, "mark-digits")},
            { Symbol.One, (new []{KnownOperations.Right, KnownOperations.PutX, KnownOperations.Right}, "mark-digits")},
            { Symbol.None, (new []{KnownOperations.Right, KnownOperations.PutZ, KnownOperations.Right, KnownOperations.Right, KnownOperations.PutR}, "find-x")},
        }),
        new("find-x", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.X, (new []{KnownOperations.Erase}, "first-r")},
            { Symbol.Schwa, (Array.Empty<Operation>(), "find-digits")},
            { Symbol.Any, (new []{KnownOperations.Left, KnownOperations.Left}, "find-x")},
        }),
        new("first-r", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.R, (new []{KnownOperations.Right, KnownOperations.Right}, "last-r")},
            { Symbol.Any, (new []{KnownOperations.Right, KnownOperations.Right}, "first-r")},
        }),
        new("last-r", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.R, (new []{KnownOperations.Right, KnownOperations.Right}, "last-r")},
            { Symbol.None, (new []{KnownOperations.PutR, KnownOperations.Right, KnownOperations.Right, KnownOperations.PutR}, "find-x")},
        }),
        new("find-digits", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.Schwa, (new []{KnownOperations.Right, KnownOperations.Right}, "find-1st-digit")},
            { Symbol.Any, (new []{KnownOperations.Left, KnownOperations.Left}, "find-digits")},
        }),
        new("find-1st-digit", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.X, (new []{KnownOperations.Left}, "found-1st-digit")},
            { Symbol.Y, (new []{KnownOperations.Left}, "found-1st-digit")},
            { Symbol.Z, (new []{KnownOperations.Left}, "found-2nd-digit")},
            { Symbol.None, (new []{KnownOperations.Right, KnownOperations.Right}, "find-1st-digit")},
        }),
        new("found-1st-digit", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.Zero, (new []{KnownOperations.Right}, "add-zero")},
            { Symbol.One, (new []{KnownOperations.Right, KnownOperations.Right, KnownOperations.Right}, "find-2nd-digit")},
        }),
        new("find-2nd-digit", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.X, (new []{KnownOperations.Left}, "found-2nd-digit")},
            { Symbol.Y, (new []{KnownOperations.Left}, "found-2nd-digit")},
            { Symbol.None, (new []{KnownOperations.Right, KnownOperations.Right}, "find-2nd-digit")},
        }),
        new("found-2nd-digit", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.Zero, (new []{KnownOperations.Right}, "add-zero")},
            { Symbol.One, (new []{KnownOperations.Right}, "add-one")},
            { Symbol.None, (new []{KnownOperations.Right}, "add-one")},
        }),
        new("add-zero", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.R, (new []{KnownOperations.PutS}, "add-finished")},
            { Symbol.U, (new []{KnownOperations.PutV}, "add-finished")},
            { Symbol.Any, (new []{KnownOperations.Right, KnownOperations.Right}, "add-zero")},
        }),
        new("add-one", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.R, (new []{KnownOperations.PutV}, "add-finished")},
            { Symbol.U, (new []{KnownOperations.PutS, KnownOperations.Right, KnownOperations.Right}, "carry")},
            { Symbol.Any, (new []{KnownOperations.Right, KnownOperations.Right}, "add-one")},
        }),
        new("carry", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.R, (new []{KnownOperations.PutU}, "add-finished")},
            { Symbol.None, (new []{KnownOperations.PutU}, "new-digit-is-zero")},
            { Symbol.U, (new []{KnownOperations.PutR, KnownOperations.Right, KnownOperations.Right}, "carry")},
        }),
        new("add-finished", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.Schwa, (new []{KnownOperations.Right, KnownOperations.Right}, "erase-old-x")},
            { Symbol.Any, (new []{KnownOperations.Left, KnownOperations.Left}, "add-finished")},
        }),
        new("erase-old-x", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.X, (new []{KnownOperations.Erase, KnownOperations.Left, KnownOperations.Left}, "print-new-x")},
            { Symbol.Z, (new []{KnownOperations.PutY, KnownOperations.Left, KnownOperations.Left}, "print-new-x")},
            { Symbol.Any, (new []{KnownOperations.Right, KnownOperations.Right}, "erase-old-x")},
        }),
        new("print-new-x", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.Schwa, (new []{KnownOperations.Right, KnownOperations.Right}, "erase-old-y")},
            { Symbol.Y, (new []{KnownOperations.PutZ}, "find-digits")},
            { Symbol.None, (new []{KnownOperations.PutX}, "find-digits")},
        }),
        new("erase-old-y", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.Y, (new []{KnownOperations.Erase, KnownOperations.Left, KnownOperations.Left}, "print-new-y")},
            { Symbol.Any, (new []{KnownOperations.Right, KnownOperations.Right}, "erase-old-y")},
        }),
        new("print-new-y", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.Schwa, (new []{KnownOperations.Right}, "new-digit-is-one")},
            { Symbol.Any, (new []{KnownOperations.PutY, KnownOperations.Right}, "reset-new-x")},
        }),
        new("reset-new-x", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.None, (new []{KnownOperations.Right, KnownOperations.PutX}, "flag-result-digits")},
            { Symbol.Any, (new []{KnownOperations.Right, KnownOperations.Right}, "reset-new-x")},
        }),
        new("flag-result-digits", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.S, (new []{KnownOperations.PutT, KnownOperations.Right, KnownOperations.Right}, "unflag-result-digits")},
            { Symbol.V, (new []{KnownOperations.PutW, KnownOperations.Right, KnownOperations.Right}, "unflag-result-digits")},
            { Symbol.Any, (new []{KnownOperations.Right, KnownOperations.Right}, "flag-result-digits")},
        }),
        new("unflag-result-digits", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.S, (new []{KnownOperations.PutR, KnownOperations.Right, KnownOperations.Right}, "unflag-result-digits")},
            { Symbol.V, (new []{KnownOperations.PutU, KnownOperations.Right, KnownOperations.Right}, "unflag-result-digits")},
            { Symbol.Any, (Array.Empty<Operation>(), "find-digits")},
        }),
        new("new-digit-is-zero", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.Schwa, (new []{KnownOperations.Right}, "print-zero-digit")},
            { Symbol.Any, (new []{KnownOperations.Left}, "new-digit-is-zero")},
        }),
        new("print-zero-digit", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.Zero, (new []{KnownOperations.Right, KnownOperations.Erase, KnownOperations.Right}, "print-zero-digit")},
            { Symbol.One, (new []{KnownOperations.Right, KnownOperations.Erase, KnownOperations.Right}, "print-zero-digit")},
            { Symbol.None, (new []{KnownOperations.PutZero, KnownOperations.Right, KnownOperations.Right, KnownOperations.Right}, "cleanup")},
        }),
        new("new-digit-is-one", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.Schwa, (new []{KnownOperations.Right}, "print-one-digit")},
            { Symbol.Any, (new []{KnownOperations.Left}, "new-digit-is-one")},
        }),
        new("print-one-digit", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.Zero, (new []{KnownOperations.Right, KnownOperations.Erase, KnownOperations.Right}, "print-one-digit")},
            { Symbol.One, (new []{KnownOperations.Right, KnownOperations.Erase, KnownOperations.Right}, "print-one-digit")},
            { Symbol.None, (new []{KnownOperations.PutOne, KnownOperations.Right, KnownOperations.Right, KnownOperations.Right}, "cleanup")},
        }),
        new("cleanup", new Dictionary<Symbol, (Operation[] operations, string finalMConfig)>()
        {
            { Symbol.None, (Array.Empty<Operation>(), "new")},
            { Symbol.Any, (new []{KnownOperations.Erase, KnownOperations.Right, KnownOperations.Right}, "cleanup")},
        }),
    };
}

public class MachineRunner
{
    public readonly struct MachineState
    {
        public Tape Tape { get; }
        public readonly string? MachineSymbol;
        public readonly Symbol? BehaviourSymbol;
        public readonly int? OperationIndex;
        public readonly bool ShouldContinue;

        public MachineState(Tape tape, Symbol? behaviourSymbol, int? operationIndex, bool shouldContinue, string? machineSymbol)
        {
            Tape = tape;
            BehaviourSymbol = behaviourSymbol;
            OperationIndex = operationIndex;
            ShouldContinue = shouldContinue;
            MachineSymbol = machineSymbol;
        }
        
        public static MachineState Empty => new(new Tape(), null, null, true, null);
        public static MachineState NewMachine(Tape tape, string machineSymbol) => new(tape, null, null, true, machineSymbol);
        public static MachineState NoContinuation(Tape tape) => new(tape, null, null, false, null);
    }
    
    private readonly Machine[] _machines;
    
    public MachineRunner(Machine[] machines)
    {
        _machines = machines;
    }

    public OneOf<MachineState, Error<string>> Move(MachineState state)
    {
        var symbolMachine = FindMachine(_machines, state.Tape.Current, state.MachineSymbol);
        if (symbolMachine == null)
        {
            return new Error<string>(state.MachineSymbol != null ? $"There is no machine with symbol: {state.MachineSymbol}" : $"There is no machine for current tape symbol: {state.Tape.Current}");
        }

        if (state.BehaviourSymbol.HasValue)
        {
            return DoBehaviour(symbolMachine, state.BehaviourSymbol.Value, state.Tape, state.OperationIndex ?? 0);
        }

        foreach (var (behaviourSymbol, _) in symbolMachine.Behaviours)
        {
            if (!CompareSymbol(behaviourSymbol, state.Tape.Current)) continue;

            return DoBehaviour(symbolMachine, behaviourSymbol, state.Tape, state.OperationIndex ?? 0);
        }

        return new Error<string>($"Any behaviour of machine with symbol {symbolMachine.MConfig} has not matched. Configuration of machine may be invalid");
    }

    private static Machine? FindMachine(Machine[] machines, char currentTapeElement, string? symbol = null)
    {
        return symbol != null ? 
            machines.SingleOrDefault(x => x.MConfig == symbol) : 
            machines.FirstOrDefault(x => x.Behaviours.Any(y => CompareSymbol(y.Key, currentTapeElement)));
    }

    private static MachineState DoBehaviour(Machine machine, Symbol behaviourSymbol, Tape tape, int operationIndex)
    {
        var behaviour = machine.Behaviours[behaviourSymbol];
        if (behaviour.operations.Length == 0)
        {
            return MachineState.NewMachine(tape, behaviour.finalMConfig);
        }

        return DoBehaviour(machine, behaviourSymbol, tape, operationIndex, behaviour);
    }

    private static MachineState DoBehaviour(Machine machine, Symbol behaviourSymbol, Tape tape, int operationIndex, (Operation[] operations, string finalMConfig) behaviour)
    {
        var shouldContinue = DoOperation(ref tape, behaviour.operations[operationIndex]);
        if (!shouldContinue)
        {
            return MachineState.NoContinuation(tape);
        }

        return behaviour.operations.Length > operationIndex + 1
            ? new MachineState(tape, behaviourSymbol, operationIndex + 1, true, machine.MConfig)
            : MachineState.NewMachine(tape, behaviour.finalMConfig);
    }

    private static bool DoOperation(ref Tape tape, Operation operation)
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
        
        return true;
    }

    private static bool CompareSymbol(Symbol symbol, char value)
    {
        return symbol switch
        {
            Symbol.One => value == KnownOperations.One,
            Symbol.Zero => value == KnownOperations.Zero,
            Symbol.ZeroOrOne => value is KnownOperations.Zero or KnownOperations.One,
            Symbol.X => value == KnownOperations.Ex,
            Symbol.Y => value == KnownOperations.Y,
            Symbol.R => value == KnownOperations.R,
            Symbol.Z => value == KnownOperations.Zet,
            Symbol.U => value == KnownOperations.U,
            Symbol.S => value == KnownOperations.S,
            Symbol.V => value == KnownOperations.V,
            Symbol.Schwa => value == KnownOperations.Schwa,
            Symbol.None => value == KnownOperations.NullTerminator,
            Symbol.Any => true,
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
    Y,
    Z,
    Schwa,
    R,
    U,
    S,
    V
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
    private const int DefaultSize = 1024;
    
    public Tape()
    {
        InternalTape = new char[DefaultSize];
        CurrentIndex = 0;
        Capacity = 0;
    }

    public int CurrentIndex { get; private set; }
    public char Current => InternalTape[CurrentIndex];
    private char[] InternalTape { get; set; }
    private int Capacity { get; set; }
    public void SetValue(char value)
    {
        InternalTape[CurrentIndex] = value;    
    }
    
    public void MoveLeft()
    {
        if (CurrentIndex == 0)
        {
            Capacity++;
            
            var tmp = new char[InternalTape.Length];
            //In case of moving left from index 0 we need to shift elements right to make place for new one, again on index 0
            //TODO: In case of performance problems we should choose different solution - like keep negative array as separate tape and join them later or to calculate some shift on runtime
            Array.Copy(InternalTape, 0, tmp, 1, Capacity);
            InternalTape = tmp;
        }
        else
        {
            CurrentIndex -= 1;   
        }
    }

    public void MoveRight()
    {
        CurrentIndex += 1;
        Capacity = Capacity > CurrentIndex ? Capacity : CurrentIndex;
        
        if (CurrentIndex >= InternalTape.Length)
        {
            var tmp = InternalTape;
            Array.Resize(ref tmp, InternalTape.Length * 2);

            InternalTape = tmp;
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

public record Operation(OperationType Type, char? Symbol = null)
{
    public string DisplayValue() => Symbol.HasValue ? $"{Type.ToString()}({Symbol})" : Type.ToString();

}
public record Machine(string MConfig, IDictionary<Symbol, (Operation[] operations, string finalMConfig)> Behaviours);