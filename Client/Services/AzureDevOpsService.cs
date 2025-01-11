using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace testauthblazor.Client.Services
{
    public class AzureDevOpsService(HttpClient httpClient, IAccessTokenProvider tokenProvider)
    {
        /// <summary>
        /// The HTTP client used for sending HTTP requests and receiving HTTP responses.
        /// </summary>
        private readonly HttpClient _httpClient = httpClient;

        /// <summary>
        /// The token provider used for acquiring access tokens for authenticated requests.
        /// </summary>
        private readonly IAccessTokenProvider _tokenProvider = tokenProvider;

        /// <summary>
        ///     Asynchronously retrieves an access token using the token provider.
        /// </summary>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains the access token as a string.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when an access token could not be acquired.
        /// </exception>
        private async Task<string> GetAccessTokenAsync()
        {
            var result = await _tokenProvider.RequestAccessToken();
            if (result.TryGetToken(out var token))
            {
                return token.Value;
            }

            throw new InvalidOperationException("Could not acquire access token.");
        }

        /// <summary>
        ///     Asynchronously retrieves a list of project names from Azure DevOps.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of project names.</returns>
        public async Task<List<string>> GetProjectsAsync()
        {
            var token = await GetAccessTokenAsync();
            string organisation = "<azure devops organisation>";
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://dev.azure.com/{organisation}/_apis/projects?api-version=7.1");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var projects = JsonDocument.Parse(content).RootElement.GetProperty("value").EnumerateArray()
                .Select(p => p.GetProperty("name").GetString()).ToList();

            return projects;
        }
    }
}
