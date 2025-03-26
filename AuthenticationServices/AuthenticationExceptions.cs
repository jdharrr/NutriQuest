namespace AuthenticationServices;

public class InvalidTokenException : Exception
{
    public InvalidTokenException() : base("Invalid Token.") { }

    public InvalidTokenException(string message) : base(message) { }
}

public class InvalidPasswordException : Exception
{
    public InvalidPasswordException() : base("Invalid Password.") { }

    public InvalidPasswordException(string message) : base(message) { }
}