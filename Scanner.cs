namespace cs_lox;
 
// Defines a class to represent the scanner/lexer.
public class Scanner 
{
    // Define a dictionary to map keywords to their corresponding token types.
    private static readonly Dictionary<string, TokenType> keywords = new()
    {
        { "and", TokenType.AND },
        { "class", TokenType.CLASS },
        { "else", TokenType.ELSE },
        { "false", TokenType.FALSE },
        { "for", TokenType.FOR },
        { "fun", TokenType.FUN },
        { "if", TokenType.IF },
        { "nil", TokenType.NIL },
        { "or", TokenType.OR },
        { "print", TokenType.PRINT },
        { "return", TokenType.RETURN },
        { "super", TokenType.SUPER },
        { "this", TokenType.THIS },
        { "true", TokenType.TRUE },
        { "var", TokenType.VAR },
        { "while", TokenType.WHILE }
    };

    private readonly string source; // Source code to scan.
    private readonly List<Token> tokens = new();     // List of the scanned tokens.
    private int current;    // Current position of the lexer in the source code.
    private int line = 1;   // Current line in the source code.                                      
    private int start;      // Start position of the current token.
    
    // Constructor to initialize the scanner with the source code.
    public Scanner(string source)
    {
        this.source = source;
    }
    
    // Method to scan and return the list of tokens.
    public List<Token> ScanTokens()
    {
        // Until the scanner has reached the end of the code.
        while (!IsAtEnd())
        {
            start = current; // Update the start position.
            ScanToken(); // Scan the next token.
        }
    
        // Add an end-of-file token at the end.
        tokens.Add(new Token(TokenType.EOF, "", null, line));
        return tokens;
    }

    // Check if the lexer has reached the end of the source code.
    private bool IsAtEnd()
    {
        return current >= source.Length;
    }

    // Scan and classify the next token.
    private void ScanToken()
    {
        var c = Advance(); // Get the next character.
        switch (c)
        {
            case '(':
                AddToken(TokenType.LEFT_PAREN);
                break;
            case ')':
                AddToken(TokenType.RIGHT_PAREN);
                break;
            case '{':
                AddToken(TokenType.LEFT_BRACE);
                break;
            case '}':
                AddToken(TokenType.RIGHT_BRACE);
                break;
            case ',':
                AddToken(TokenType.COMMA);
                break;
            case '.':
                AddToken(TokenType.DOT);
                break;
            case '-':
                AddToken(TokenType.MINUS);
                break;
            case '+':
                AddToken(TokenType.PLUS);
                break;
            case ';':
                AddToken(TokenType.SEMICOLON);
                break;
            case '*':
                AddToken(TokenType.STAR);
                break;
            case '!':
                AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                break;
            case '=':
                AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '<':
                AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                break;
            case '>':
                AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;
            case '/':
                if (Match('/')) // Handle comment.
                    while (Peek() != '\n' && !IsAtEnd())
                        Advance();
                else
                    AddToken(TokenType.SLASH);
                break;
            case ' ': // Ignore white space
            case '\r':
            case '\t':
                break;
            case '\n': // New line means to increment the line counter.
                line++;
                break;
            case '"': // Handle string literal.
                String();
                break;
            default:
                if (IsDigit(c)) // Handle number literal.
                    Number();
                else if (IsAlpha(c))
                    Identifier();
                else
                    Lox.Error(line, "Unexpected character.");
                break;
        }
    }

    // Advance to the next charcter and return the current character.
    private char Advance()
    {
        current++;
        return source[current - 1];
    }

    // Add a token to the list of tokens.
    private void AddToken(TokenType type, object literal = null)
    {
        var text = source.Substring(start, current - start); // Get the token text.
        if (type == TokenType.IDENTIFIER && keywords.ContainsKey(text)) type = keywords[text]; // Check if its a keyword.
        tokens.Add(new Token(type, text, literal, line)); // Add the token.
    }

    // Check of the next character matches the expected character.
    private bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (source[current] != expected) return false;

        current++;
        return true;
    }

    // Peek at the current character without advancing.
    private char Peek()
    {
        if (IsAtEnd()) return '\0';
        return source[current];
    }

    // Peek at the next character without advancing.
    private char PeekNext()
    {
        if (current + 1 >= source.Length) return '\0';
        return source[current + 1];
    }

    // Handle string literals.
    private void String()
    {
        // While the current character isn't ", and the lexer hasn't reached the end of the file.
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n') line++; // If the current character is TokenType new line, increment the current line.
            Advance(); // Advance to the next character in the source code.
        }

        // If the lexer has reached the end of the source code.
        if (IsAtEnd())
        {
            // And the string in the source code doesn't have a closing, then give an error.
            Lox.Error(line, "Unterminated string.");
            return;
        }

        Advance(); // Consume the closing "
        // "start + 1" skips the initial opening quote, and "current - start - 2" excludes the end quote.
        var value = source.Substring(start + 1, current - start - 2); // get the string value.
        AddToken(TokenType.STRING, value); // Add the string token.
    }

    // Handles number literals.
    private void Number()
    {
        while (IsDigit(Peek())) Advance(); // Consume the digits.

        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            Advance(); // Consume the .
            while (IsDigit(Peek())) Advance(); // Consume the fractional part.
        }

        AddToken(TokenType.NUMBER, double.Parse(source.Substring(start, current - start))); // Add the number token.
    }

    // Handles identifiers and keywords.
    private void Identifier()
    {
        while (IsAlphaNumeric(Peek())) Advance(); // Consume the identifier.

        var text = source.Substring(start, current - start); // Get the identifier text.
        TokenType type;
        if (!keywords.TryGetValue(text, out type)) type = TokenType.IDENTIFIER; // Determine if its a keyword.

        AddToken(type); // Add the identifier or keyword token.
    }

    // Check if a character is a digit.
    private bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    // Check if a character is a letter or underscore.
    private bool IsAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
    }

    // Returns whether the token is a number or a letter or an underscore.
    private bool IsAlphaNumeric(char c)
    {
        return IsAlpha(c) || IsDigit(c);
    }
}