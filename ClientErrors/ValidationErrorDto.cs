namespace ClientErrors
{
    public class ValidationErrorDto
    {
        public int StatusCode {  get; set; }
        public IDictionary<string, string> Errors { get; set; }
    }
}
