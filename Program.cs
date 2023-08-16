using Behaviour = System.Collections.Generic.IDictionary<Symbol, (Operation[] operations, char finalMConfig)>;

const char schwa = 'e';
var putSchwa = new Operation(OperationType.Put, schwa);
var putOne = new Operation(OperationType.Put, '0');
var putZero = new Operation(OperationType.Put, '1');
var putX = new Operation(OperationType.Put, 'x');
var right = new Operation(OperationType.Right);
var left = new Operation(OperationType.Left);


var machine = new Machine[]
{
    new('b', new Dictionary<Symbol, (Operation[], char)>()
    {
        { Symbol.Any , (new []{putSchwa, right, putSchwa, right, putZero, right, right, putZero, left, left}, 'o')},
        
    }),
    new('o', new Dictionary<Symbol, (Operation[], char)>()
    {
        { Symbol.One , (new []{right, putX, left, left, left}, 'o')},
        { Symbol.Zero , (Array.Empty<Operation>(), 'q')},
    }),
    new('q', new Dictionary<Symbol, (Operation[], char)>()
    {
        { Symbol.ZeroOrOne , (new []{right, right}, 'q')},
        { Symbol.None , (new []{putOne, left}, 'p')},
    }),
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
    Right,
    Left,
    Put,
    Erase,
    Halt
}

record Operation(OperationType Type, char? SymbolToPut = null);
record Machine(char MConfig, Behaviour Behaviours);
