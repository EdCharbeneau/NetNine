using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NetNine.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<WebLLMService>();
await builder.Build().RunAsync();