using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http;

namespace CodeLifeAcademy.Client.Services;

public class TokenRefreshHandler: DelegatingHandler
{
    private readonly NavigationManager _navigationManager;
    private readonly HttpClient _http;

    public TokenRefreshHandler(NavigationManager navigationManager, HttpClient http)
    {
        _navigationManager = navigationManager;
        _http = http;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            var refreshResponse = await _http.PostAsync("api/auth/refresh", null);

            if (refreshResponse.IsSuccessStatusCode)
            {
                // Пробуем повторить оригинальный запрос
                request.Headers.Remove("Authorization"); // Если использовался
                return await base.SendAsync(request, cancellationToken);
            }
        }

        return response;
    }
}
