using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Soil.Server.Services.LocalStorageAuthenticationStateProvider;

public class LocalStorageAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;

    public LocalStorageAuthenticationStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        var userRole = await _localStorage.GetItemAsync<string>("userRole");
        var userName = await _localStorage.GetItemAsync<string>("userName");

        ClaimsIdentity identity;

        if (!string.IsNullOrWhiteSpace(token))
        {
            identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, userName ?? "User"),
                new Claim(ClaimTypes.Role, userRole ?? "User")
            }, "jwt");
        }
        else
        {
            identity = new ClaimsIdentity();
        }

        var user = new ClaimsPrincipal(identity);
        return new AuthenticationState(user);
    }

    public void NotifyAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}