namespace cs_lox;
  
// Defines a class to represent a Token.
public class Token
{
    // Constructor to initialize a token with its parameters.
    public Token(TokenType type, string lexeme, object literal, int line)
    {
        Type = type;            // Sets the token's type.
        Lexeme = lexeme;        // Set the lexeme, the actual text broken into its distinguishable words.
        Literal = literal;      // Stores the value of a variable, expression, or command's input.
        Line = line;            // Set the line number where the token appears, for error accuracy.
    }

    // Properties to get the values of the token's attributes.
    public TokenType Type { get; }          // Gets the value of type (e.g., identifier, keyword, symbol).
    public string Lexeme { get; }           // Gets the value of lexeme (e.g., int, var, =).
    public object Literal { get; }          // Gets the value (e.g. 23, -34, Hello).
    public int Line { get; }                // Gets the line that the current lexeme is on.

    // Overrides the ToString method, to provide a string representation of the token.
    public override string ToString()
    {
        return $"{Type} {Lexeme} {Literal}";
    }
}