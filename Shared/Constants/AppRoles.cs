namespace Shared.Constants
{
    public static class AppRoles
    {
        public const string Admin = "Administrator";
        public const string AuthService = "InternalAuthService";
        public static IEnumerable<string> AllUserRoles()
            => [Admin];
        public static IEnumerable<string> AllServiceRoles()
            => [AuthService];
    }
}
