namespace ClientErrors
{
    public class ClientError: ClientSideException
    {
        public ClientError(string error)
        {
            Error = error;
        }

        public string Error {  get; init; }
    }
}
