using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.Text.Json;
using CodeLifeAcademy.Core.Entities;

/*
 * Blazor WASM не знает автоматически, авторизован пользователь или нет, создаем кастомный провайдер
 * на сторое клиента, чтобы понять какие у него Claims и передать в Blazor для управления
 */
public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _http;

    public CustomAuthStateProvider(HttpClient http)
    {
        _http = http;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var response = await _http.GetAsync("users/userinfo"); // endpoint, который отдаёт инфу о текущем пользователе
            if (!response.IsSuccessStatusCode)
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            var json = await response.Content.ReadAsStringAsync();
            var userInfo = JsonSerializer.Deserialize<UserInfo>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var claims = userInfo.Claims.Select(c => new Claim(c.Type, c.Value));
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
        catch
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    //Вызывается после логина, чтобы обновить состояние Blazor
    public void NotifyUserAuthentication(ClaimsPrincipal user)
    {
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    //Вызывается после выхода
    public void NotifyUserLogout()
    {
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
    }
}
