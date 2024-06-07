// This is the main access point for this transpiler, which defines how the code is able to access incoming commands.

// All of the code is kept within the cs_lox namespace. This would be useful if this wasn't a learning project, 
// but never the less its still good practice.
namespace cs_lox;
 
public class Lox
{ 
    // Create a single instance of the Intepreter class.
    private static readonly Interpreter interpreter = new();
    
    // Property to check if an error has occured
    public static bool HadError { get; private set; }

    // The main entry point of the program.
    public static void Main(string[] args)
    {
        // Check if more than one argument is passed. 
        if (args.Length > 1)
        {
            // If the arguments are more than one, then display an error code.
            Console.WriteLine("Usage: lox [script]");
            Environment.Exit(64);
        }
        else if (args.Length == 1)
        {
            // If only one argument is passed, run the file.
            RunFile(args[0]);
        }
        else
        {
            // If no arguments are passed, start the interactive prompt through terminal.
            RunPrompt();
        }
    }

    // Method to run a script file.
    private static void RunFile(string path)
    {
        // Read the file's contents into a string.
        var script = File.ReadAllText(path);
        // Execute the script.
        Run(script);
        
        // Exit with an error code if there is an error within the script.
        if (HadError) Environment.Exit(65);
    }

    // Method to run an interactive prompt through the terminal.
    private static void RunPrompt()
    {
        // Display the prompt and read the input from the user.
        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine();
            if (line == null) break; // Exit if the input is null.
            Run(line); // Execute the input line.
            HadError = false; // Reset the error flag after each line.
        }
    }

    // Method to execute a given source code.
    private static void Run(string source)
    {
        // Initialize the scanner with the source code and get tokens.
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();

        // Initialize the parser with the tokens and parse the expression.
        var parser = new Parser(tokens);
        var expression = parser.Parse();

        // If there was an error, return early.
        if (HadError) return;
        
        // Interpret the parsed expression.
        interpreter.Interpret(expression);
    }

    // Method to report an error with a specific line number.
    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    // Helper method to print error messages and set the error flag.
    private static void Report(int line, string where, string message)
    {
        Console.WriteLine($"[line {line}] Error{where}: {message}");
        HadError = true;
    }
    
    // Method to report a runtime error with a specific token.
    public static void RuntimeError(RuntimeError error)
    {
        Console.WriteLine($"{error.Message}\n[line {error.Token.Line}]");
        HadError = true;
    }
}