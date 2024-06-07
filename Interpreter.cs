namespace cs_lox;

// Interpreter class responsible for interpreting Lox expressions
public class Interpreter : Expr.IVisitor<object>
{
    // VisitLiteralExpr method: handles literal expressions
    public object VisitLiteralExpr(Expr.Literal expr)
    {
        return expr.Value; // Return the value of the literal expression
    }

    // VisitGroupingExpr method: handles grouping expressions
    public object VisitGroupingExpr(Expr.Grouping expr)
    {
        return Evaluate(expr.Expression); // Recursively evaluate the grouped expression
    }

    // VisitUnaryExpr method: handles unary expressions
    public object VisitUnaryExpr(Expr.Unary expr)
    {
        var right = Evaluate(expr.Right); // Evaluate the right side of the unary expression

        // Perform different operations based on the type of unary operator
        switch (expr.Operator.Type)
        {
            case TokenType.BANG:
                return !IsTruthy(right); // Negate the truthiness of the expression
            case TokenType.MINUS:
                return -(double)right; // Negate the numeric value
        }

        // Unreachable code
        return null;
    }

    // VisitBinaryExpr method: handles binary expressions
    public object VisitBinaryExpr(Expr.Binary expr)
    {
        var left = Evaluate(expr.Left); // Evaluate the left side of the binary expression
        var right = Evaluate(expr.Right); // Evaluate the right side of the binary expression

        // Perform different operations based on the type of binary operator
        switch (expr.Operator.Type)
        {
            case TokenType.GREATER:
                return (double)left > (double)right; // Check if left is greater than right
            case TokenType.GREATER_EQUAL:
                return (double)left >= (double)right; // Check if left is greater than or equal to right
            case TokenType.LESS:
                return (double)left < (double)right; // Check if left is less than right
            case TokenType.LESS_EQUAL:
                return (double)left <= (double)right; // Check if left is less than or equal to right
            case TokenType.BANG_EQUAL:
                return !IsEqual(left, right); // Check if left is not equal to right
            case TokenType.EQUAL_EQUAL:
                return IsEqual(left, right); // Check if left is equal to right
            case TokenType.MINUS:
                return (double)left - (double)right; // Subtract right from left
            case TokenType.PLUS:
                if (left is double && right is double) return (double)left + (double)right; // Add two numbers
                if (left is string && right is string) return (string)left + (string)right; // Concatenate two strings
                break;
            case TokenType.SLASH:
                return (double)left / (double)right; // Divide left by right
            case TokenType.STAR:
                return (double)left * (double)right; // Multiply left by right
        }

        // Unreachable code
        return null;
    }

    // Interpret method: interprets an expression and prints the result
    public void Interpret(Expr expression)
    {
        try
        {
            var value = Evaluate(expression); // Evaluate the expression
            Console.WriteLine(Stringify(value)); // Print the result
        }
        catch (RuntimeError error)
        {
            Lox.RuntimeError(error); // Handle runtime errors
        }
    }

    // Evaluate method: evaluates an expression by accepting a visitor
    private object Evaluate(Expr expr)
    {
        return expr.Accept(this); // Accept the visitor for the expression
    }

    // IsTruthy method: checks if an object is truthy
    private bool IsTruthy(object obj)
    {
        if (obj == null) return false; // Null values are falsy
        if (obj is bool) return (bool)obj; // Booleans are truthy if true
        return true; // Other values are truthy
    }

    // IsEqual method: checks if two objects are equal
    private bool IsEqual(object a, object b)
    {
        if (a == null && b == null) return true; // Null values are equal
        if (a == null) return false; // Null values are not equal to non-null values
        return a.Equals(b); // Check equality using the Equals method
    }

    // Stringify method: converts an object to its string representation
    private string Stringify(object obj)
    {
        if (obj == null) return "nil"; // Convert null values to "nil"

        if (obj is double)
        {
            var text = obj.ToString(); // Convert double to string
            if (text.EndsWith(".0")) text = text.Substring(0, text.Length - 2); // Remove trailing .0 for integers
            return text; // Return the string representation
        }

        return obj.ToString(); // Convert other types to string
    }
}

// RuntimeError class: represents runtime errors during interpretation
public class RuntimeError : Exception
{
    public RuntimeError(Token token, string message) : base(message)
    {
        Token = token; // Store the token associated with the error
    }

    public Token Token { get; } // Token associated with the error
}
