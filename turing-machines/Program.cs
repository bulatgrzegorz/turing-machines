using System.Collections;
using OneOf;
using OneOf.Types;

namespace turing_machines;

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
            { Symbol.None, (new[] { KnownOperations.PutSchwa, KnownOperations.Right, KnownOperations.PutSchwa, KnownOperations.Right, KnownOperations.PutZero, KnownOperations.Right, KnownOperations.Right, KnownOperations.PutZero, KnownOperations.Left, KnownOperations.Left }, 'o') },
        }),
        new('o', new Dictionary<Symbol, (Operation[], char)>()
        {
            { Symbol.One, (new[] { KnownOperations.Right, KnownOperations.PutX, KnownOperations.Left, KnownOperations.Left, KnownOperations.Left }, 'o') },
            { Symbol.Zero, (Array.Empty<Operation>(), 'q') },
        }),
        new('q', new Dictionary<Symbol, (Operation[], char)>()
        {
            { Symbol.ZeroOrOne, (new[] { KnownOperations.Right, KnownOperations.Right }, 'q') },
            { Symbol.None, (new[] { KnownOperations.PutOne, KnownOperations.Left }, 'p') },
        }),
        new('p', new Dictionary<Symbol, (Operation[], char)>()
        {
            { Symbol.X, (new[] { KnownOperations.Erase, KnownOperations.Right }, 'q') },
            { Symbol.Schwa, (new[] { KnownOperations.Right }, 'f') },
            { Symbol.None, (new[] { KnownOperations.Left, KnownOperations.Left }, 'p') },
        }),
        new('f', new Dictionary<Symbol, (Operation[], char)>()
        {
            { Symbol.Any, (new[] { KnownOperations.Right, KnownOperations.Right }, 'f') },
            { Symbol.None, (new[] { KnownOperations.PutZero, KnownOperations.Left, KnownOperations.Left }, 'o') },
        }),
    };

    public static readonly Machine[] Something =
    {
        new('b', new Dictionary<Symbol, (Operation[] operations, char finalMConfig)>()
        {
            { Symbol.None, (new[] { KnownOperations.PutZero }, 'i') }
        }),
        new('i', new Dictionary<Symbol, (Operation[] operations, char finalMConfig)>()
        {
            { Symbol.Zero, (new[] { KnownOperations.PutOne }, 'r') },
            { Symbol.One, (new[] { KnownOperations.PutZero, KnownOperations.Left }, 'i') },
            { Symbol.None, (new[] { KnownOperations.PutOne }, 'r') },
        }),
        new('r', new Dictionary<Symbol, (Operation[] operations, char finalMConfig)>()
        {
            { Symbol.None, (new[] { KnownOperations.Left }, 'i') },
            { Symbol.Any, (new[] { KnownOperations.Right }, 'r') },
        }),
    };
}

public class MachineRunner
{
    public readonly struct MachineState
    {
        public Tape Tape { get; }
        public readonly char? MachineSymbol;
        public readonly Symbol? BehaviourSymbol;
        public readonly int? OperationIndex;
        public readonly bool ShouldContinue;

        public MachineState(Tape tape, Symbol? behaviourSymbol, int? operationIndex, bool shouldContinue, char? machineSymbol)
        {
            Tape = tape;
            BehaviourSymbol = behaviourSymbol;
            OperationIndex = operationIndex;
            ShouldContinue = shouldContinue;
            MachineSymbol = machineSymbol;
        }
        
        public static MachineState Empty => new(new Tape(), null, null, true, null);
        public static MachineState NewMachine(Tape tape, char machineSymbol) => new(tape, null, null, true, machineSymbol);
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
            return new Error<string>(state.MachineSymbol.HasValue ? $"There is no machine with symbol: {state.MachineSymbol.Value}" : $"There is no machine for current tape symbol: {state.Tape.Current}");
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

        return new Error<string>($"Any behaviour of machine with symbol {symbolMachine.MConfig} has matched. Configuration of machine may be invalid");
    }

    private static Machine? FindMachine(Machine[] machines, char currentTapeElement, char? symbol = null)
    {
        return symbol.HasValue ? 
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

    private static MachineState DoBehaviour(Machine machine, Symbol behaviourSymbol, Tape tape, int operationIndex, (Operation[] operations, char finalMConfig) behaviour)
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
            Symbol.Schwa => value == KnownOperations.Schwa,
            Symbol.None => value == KnownOperations.NullTerminator,
            Symbol.Any => value != KnownOperations.NullTerminator,
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

    public IEnumerator<char> GetEnumerator()
    {
        for (var i = 0; i < Capacity + 1; i++)
        {
            yield return InternalTape[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public record Operation(OperationType Type, char? Symbol = null);
public record Machine(char MConfig, IDictionary<Symbol, (Operation[] operations, char finalMConfig)> Behaviours);