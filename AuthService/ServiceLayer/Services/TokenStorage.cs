namespace ServiceLayer.Services
{
    public class TokenStorage
    {
        private readonly Lock _locker = new();
        private string _token;

        public void UpdateToken(string token)
        {
            lock (_locker)
            {
                _token = token;
            }
        }

        public string GetToken()
        {
            lock (_locker)
            {
                return _token;
            }
        }
    }
}
