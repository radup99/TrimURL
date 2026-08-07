
using System.Net.Http.Json;
using TrimUrlApi.IntegrationTests.Infrastructure;

namespace TrimUrlApi.IntegrationTests.Helpers
{
    public static class AuthenticationHelpers
    {
        public static async Task<string?> LoginAndGetToken(this HttpClient client, string username, string password)
        {
            var response = await client.PostAsJsonAsync(
                ApiRoutes.Login,
                new
                {
                    Username = username,
                    Password = password
                }
             );

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null;
        }
    }
}
