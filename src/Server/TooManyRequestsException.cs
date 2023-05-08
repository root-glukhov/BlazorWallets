namespace BlazorWallets.Server;

public class TooManyRequestsException : Exception
{
    public TooManyRequestsException() 
        : base("Превышен лимит запросов в секунду.") { }
}
