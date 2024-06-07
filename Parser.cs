namespace cs_lox;

// Define a class to represetn the parser.
public class Parser
{
    private readonly List<Token> tokens; // List the tokens to parse.
    private int current; // Index of the current token being parsed.

    // Constructor to initialize the parser with the list of tokens.
    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    // Method to start parsing and return the parsed expression.
    public Expr Parse()
    {
        try
        {
            return Expression(); // Begin parsing the expression.
        }
        catch (ParseError error) // Catch any parsing errors.
        {
            return null;
        }
    }

    // Parses the expression.
    private Expr Expression()
    {
        return Equality(); // Parse a comparison expression.
    }

    // Parse an equality expression.
    private Expr Equality()
    {
        var expr = Comparison(); // Parse a comparison expression.
        
        // Continue parsing equality expressions if there are chained equality operators.
        while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
        {
            var operatorToken = Previous(); // Get the operator token.
            var right = Comparison(); // Parse the right-hand side expression.
            expr = new Expr.Binary(expr, operatorToken, right); // Create a binary expression.
        }

        return expr; // return the parsed expression.
    }

    // Parse a comparison expression.
    private Expr Comparison()
    {
        var expr = Term(); // Parse a term expression.

        // Continue parsing comparison expressions if there are chained comparison operators.
        while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
        {
            var operatorToken = Previous();
            var right = Term();
            expr = new Expr.Binary(expr, operatorToken, right);
        }

        return expr;
    }

    private Expr Term()
    {
        var expr = Factor();

        while (Match(TokenType.MINUS, TokenType.PLUS))
        {
            var operatorToken = Previous();
            var right = Factor();
            expr = new Expr.Binary(expr, operatorToken, right);
        }

        return expr;
    }

    private Expr Factor()
    {
        var expr = Unary();

        while (Match(TokenType.SLASH, TokenType.STAR))
        {
            var operatorToken = Previous();
            var right = Unary();
            expr = new Expr.Binary(expr, operatorToken, right);
        }

        return expr;
    }

    private Expr Unary()
    {
        if (Match(TokenType.BANG, TokenType.MINUS))
        {
            var operatorToken = Previous();
            var right = Unary();
            return new Expr.Unary(operatorToken, right);
        }

        return Primary();
    }

    private Expr Primary()
    {
        if (Match(TokenType.FALSE)) return new Expr.Literal(false);
        if (Match(TokenType.TRUE)) return new Expr.Literal(true);
        if (Match(TokenType.NIL)) return new Expr.Literal(null);

        if (Match(TokenType.NUMBER, TokenType.STRING)) return new Expr.Literal(Previous().Literal);

        if (Match(TokenType.LEFT_PAREN))
        {
            var expr = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
            return new Expr.Grouping(expr);
        }

        throw Error(Peek(), "Expect expression.");
    }

    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
            if (Check(type))
            {
                Advance();
                return true;
            }

        return false;
    }

    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }
 
    private Token Advance()
    {
        if (!IsAtEnd()) current++;
        return Previous();
    } 

    private bool IsAtEnd()
    {
        return Peek().Type == TokenType.EOF;
    }

    private Token Peek()
    {
        return tokens[current];
    }

    private Token Previous()
    {
        return tokens[current - 1];
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type)) return Advance();

        throw Error(Peek(), message);
    }

    private ParseError Error(Token token, string message)
    {
        Lox.Error(token.Line, message);
        return new ParseError();
    }
 
    private void Synchronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            if (Previous().Type == TokenType.SEMICOLON) return;

            switch (Peek().Type)
            {
                case TokenType.CLASS:
                case TokenType.FUN:
                case TokenType.VAR:
                case TokenType.FOR:
                case TokenType.IF:
                case TokenType.WHILE:
                case TokenType.PRINT:
                case TokenType.RETURN:
                    return;
            }

            Advance();
        }
    }

    private class ParseError : Exception
    {
    }
}