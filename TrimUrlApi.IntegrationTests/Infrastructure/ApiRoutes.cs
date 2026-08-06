namespace TrimUrlApi.IntegrationTests.Infrastructure
{
    public static class ApiRoutes
    {
        public const string Users = "/users";

        public const string AuthenticatedUser = $"{Users}/me";

        public const string ShortUrls = "/short-urls";

        public static string ShortUrlByCode(string code) 
            => $"{ShortUrls}/code/{code}";

        public static string ShortUrlByCodeAsAdmin(string code)
            => $"{ShortUrls}/admin/code/{code}";

        public const string ShortUrlsFromAuthUser = $"{ShortUrls}/me";

        public const string Login = "/login";
    }
}
