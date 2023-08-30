using Behaviour = System.Collections.Generic.IDictionary<Symbol, (Operation[] operations, char finalMConfig)>;

const char schwa = 'e';
const char ex = 'x';
const char one = '1';
const char zero = '0';
const char nullTerminator = '\0';
var putSchwa = new Operation(OperationType.Put, schwa);
var putOne = new Operation(OperationType.Put, one);
var putZero = new Operation(OperationType.Put, zero);
var putX = new Operation(OperationType.Put, ex);
var right = new Operation(OperationType.MoveRight);
var left = new Operation(OperationType.MoveLeft);
var erase = new Operation(OperationType.Erase);

var machine = new Machine[]
{
    new('b', new Dictionary<Symbol, (Operation[], char)>()
    {
        { Symbol.None, (new[] { putSchwa, right, putSchwa, right, putZero, right, right, putZero, left, left }, 'o') },
    }),
    new('o', new Dictionary<Symbol, (Operation[], char)>()
    {
        { Symbol.One, (new[] { right, putX, left, left, left }, 'o') },
        { Symbol.Zero, (Array.Empty<Operation>(), 'q') },
    }),
    new('q', new Dictionary<Symbol, (Operation[], char)>()
    {
        { Symbol.ZeroOrOne, (new[] { right, right }, 'q') },
        { Symbol.None, (new[] { putOne, left }, 'p') },
    }),
    new('p', new Dictionary<Symbol, (Operation[], char)>()
    {
        { Symbol.X, (new[] { erase, right }, 'q') },
        { Symbol.Schwa, (new[] { right }, 'f') },
        { Symbol.None, (new[] { left, left }, 'p') },
    }),
    new('f', new Dictionary<Symbol, (Operation[], char)>()
    {
        { Symbol.Any, (new[] { right, right }, 'f') },
        { Symbol.None, (new[] { putZero, left, left }, 'o') },
    }),
};

var symbol = 'b';
var tape = new Tape();
while (true)
{
    var symbolMachine = machine.Single(x => x.MConfig == symbol);

    foreach (var (x, y) in symbolMachine.Behaviours)
    {
        if (CompareSymbol(x, tape.Current))
        {
            var shouldContinue = DoOperations(ref tape, y.operations);
            if (!shouldContinue)
            {
                return 0;
            }

            symbol = y.finalMConfig;

            break;
        }
    }
}

static bool DoOperations(ref Tape tape, Operation[] operations)
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

        tape.Print();
        Console.SetCursorPosition(0, 0);
    }

    return true;
}

static bool CompareSymbol(Symbol symbol, char value)
{
    return symbol switch
    {
        Symbol.One => value == one,
        Symbol.Zero => value == zero,
        Symbol.ZeroOrOne => value is zero or one,
        Symbol.X => value == ex,
        Symbol.Schwa => value == schwa,
        Symbol.None => value == nullTerminator,
        Symbol.Any => value != nullTerminator,
        _ => throw new ArgumentOutOfRangeException(nameof(symbol), symbol, null)
    };
}

enum Symbol
{
    One,
    Zero,
    ZeroOrOne,
    Any,
    None,
    X,
    Schwa
}

enum OperationType
{
    MoveRight,
    MoveLeft,
    Put,
    Erase,
    Halt
}

internal ref struct Tape
{
    private const int DefaultSize = 1024;
    
    public Tape()
    {
        InternalTape = new char[DefaultSize];
        CurrentIndex = 0;
        Capacity = 0;
    }
    private char[] InternalTape { get; set; }
    private int CurrentIndex { get; set; }
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
}

internal record Operation(OperationType Type, char? Symbol = null);
internal record Machine(char MConfig, Behaviour Behaviours);
