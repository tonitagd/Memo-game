using System;

public class GameException : Exception //this is our exception. It should be used when we deal with an inappropriate behavior. It is the same as the other exceptions
{
    public GameException() : base() { }
    public GameException(string message) : base(message) { }
    public GameException(string message, Exception inner) : base(message, inner) { }
}