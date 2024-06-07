namespace cs_lox;
  
// Define an abstract class representing expressions in the Lox language
public abstract class Expr
{ 
    // Define a method for accepting a visitor that operates on expressions
    public abstract T Accept<T>(IVisitor<T> visitor);

    // Define an interface for visitors of expressions
    public interface IVisitor<T>
    {
        T VisitBinaryExpr(Binary expr); // Visit method for binary expressions
        T VisitGroupingExpr(Grouping expr); // Visit method for grouping expressions
        T VisitLiteralExpr(Literal expr); // Visit method for literal expressions
        T VisitUnaryExpr(Unary expr); // Visit method for unary expressions
    } 

    // Define a class representing binary expressions
    public class Binary : Expr
    {
        // Constructor for binary expressions
        public Binary(Expr left, Token operatorToken, Expr right)
        {
            Left = left; // Left operand of the binary expression
            Operator = operatorToken; // Operator token (e.g., +, -, *, /)
            Right = right; // Right operand of the binary expression
        }

        public Expr Left { get; } // Get the left operand
        public Token Operator { get; } // Get the operator token
        public Expr Right { get; } // Get the right operand

        // Accept method for binary expressions, invokes the appropriate visit method of the visitor
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitBinaryExpr(this); // Invoke the visit method for binary expressions
        }
    }

    // Define a class representing grouping expressions (expressions enclosed in parentheses)
    public class Grouping : Expr
    {
        // Constructor for grouping expressions
        public Grouping(Expr expression)
        {
            Expression = expression; // The expression enclosed in parentheses
        }

        public Expr Expression { get; } // Get the expression enclosed in parentheses

        // Accept method for grouping expressions, invokes the appropriate visit method of the visitor
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitGroupingExpr(this); // Invoke the visit method for grouping expressions
        }
    }

    // Define a class representing literal expressions (e.g., numbers, strings, booleans)
    public class Literal : Expr
    {
        // Constructor for literal expressions
        public Literal(object value)
        {
            Value = value; // The value of the literal expression
        }

        public object Value { get; } // Get the value of the literal expression

        // Accept method for literal expressions, invokes the appropriate visit method of the visitor
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitLiteralExpr(this); // Invoke the visit method for literal expressions
        }
    }

    // Define a class representing unary expressions (e.g., negation, logical not)
    public class Unary : Expr
    {
        // Constructor for unary expressions
        public Unary(Token operatorToken, Expr right)
        {
            Operator = operatorToken; // The operator token (e.g., -, !)
            Right = right; // The expression on which the operator is applied
        }

        public Token Operator { get; } // Get the operator token
        public Expr Right { get; } // Get the expression on which the operator is applied

        // Accept method for unary expressions, invokes the appropriate visit method of the visitor
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitUnaryExpr(this); // Invoke the visit method for unary expressions
        }
    }
}
