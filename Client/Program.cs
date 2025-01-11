using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using testauthblazor.Client;
using testauthblazor.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("testauthblazor.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("testauthblazor.ServerAPI"));
builder.Services.AddScoped<AzureDevOpsService>();

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add("499b84ac-1321-427f-aa17-267ca6975798/vso.build");
    options.ProviderOptions.DefaultAccessTokenScopes.Add("499b84ac-1321-427f-aa17-267ca6975798/vso.code_write");
    options.ProviderOptions.DefaultAccessTokenScopes.Add("499b84ac-1321-427f-aa17-267ca6975798/vso.profile");
    options.ProviderOptions.DefaultAccessTokenScopes.Add("499b84ac-1321-427f-aa17-267ca6975798/vso.work_write");
});
await builder.Build().RunAsync();
