using ClientErrors;
using Microsoft.Extensions.Options;
using ServiceLayer.Configuration;
using Shared.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ServiceLayer.Services.Authentication.Concrete
{
    public class SignInService : ISignInService
    {
        private readonly HttpClient _httpClient;

        public SignInService(TokenStorage tokenStorage, IHttpClientFactory httpClientFactory, IOptionsSnapshot<UrlsSettings> urlsSettingsSnapshot)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(urlsSettingsSnapshot.Value.UserService);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenStorage.GetToken());
        }

        public async Task<UserDto> SignUserAsync(string username, string password, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = new(HttpMethod.Post, $"auth")
            {
                Content = JsonContent.Create(new AuthRequest()
                {
                    UserName = username,
                    Password = password
                })
            };

            HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    Stream errorContentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    ClientErrorDto errorDto = await JsonSerializer.DeserializeAsync<ClientErrorDto>(errorContentStream, JsonSerializerOptions.Web, cancellationToken: cancellationToken)
                        ?? throw new Exception("Deserialization fail");
                    throw new ClientError(errorDto.Error);
                }
                else
                {
                    throw new Exception($"Request fail with status {response.StatusCode}");
                }
            }

            Stream contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            UserDto user = await JsonSerializer.DeserializeAsync<UserDto>(contentStream, JsonSerializerOptions.Web, cancellationToken: cancellationToken)
                ?? throw new Exception("Deserialization fail");

            return user;
        }
    }
}
