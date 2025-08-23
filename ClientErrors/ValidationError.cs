namespace ClientErrors
{
    public class ValidationError : ClientSideException
    {
        public ValidationError(IDictionary<string, string> errors)
        {
            Errors = errors;
        }

        public IDictionary<string, string> Errors { get; init; }
    }
}
