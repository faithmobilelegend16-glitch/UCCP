using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using Soil.Client;
using Soil.Client.Services.LocalStorageAuthenticationStateProvider;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddRadzenComponents();
// Add LocalStorage service
builder.Services.AddBlazoredLocalStorage();


// Authorization
builder.Services.AddAuthorizationCore();

// Custom AuthenticationStateProvider
builder.Services.AddScoped<AuthenticationStateProvider, LocalStorageAuthenticationStateProvider>();


await builder.Build().RunAsync();
